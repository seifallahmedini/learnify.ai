using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Models;
using OpenAI.Chat;
using Azure;
using Azure.AI.OpenAI;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Service for integrating with Learnify.ai MCP Server and Azure OpenAI
/// Provides educational AI capabilities through conversational interface
/// Uses stdio process communication similar to Claude Desktop MCP integration
/// </summary>
public class LearnifyMcpService : IDisposable
{
    private readonly ILogger<LearnifyMcpService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    private Process? _mcpServerProcess;
    private StreamWriter? _mcpWriter;
    private StreamReader? _mcpReader;
    private bool _isInitialized;
    private bool _disposed;
    
    private readonly string _mcpServerPath;
    private readonly string _mcpServerExecutable;
    private int _messageId = 1;

    public LearnifyMcpService(
        ILogger<LearnifyMcpService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // Configure MCP server path with better resolution
        _mcpServerPath = _configuration["McpServer:Path"];
        
        if (string.IsNullOrEmpty(_mcpServerPath))
        {
            // Default fallback path
            _mcpServerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "learnify.ai.mcp.server");
        }

        // Convert to absolute path if it's relative
        if (!Path.IsPathRooted(_mcpServerPath))
        {
            _mcpServerPath = Path.GetFullPath(_mcpServerPath);
        }
        
        _mcpServerExecutable = _configuration["McpServer:Executable"] ?? "dotnet";
        
        _logger.LogInformation("MCP Server configured at: {Path}", _mcpServerPath);
    }

    /// <summary>
    /// Initialize the MCP client connection to the server
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            _logger.LogInformation("MCP Service already initialized");
            return;
        }

        try
        {
            _logger.LogInformation("Initializing MCP Service...");
            
            // Start MCP server process
            await StartMcpServerProcessAsync();
            
            // Initialize MCP connection
            await InitializeMcpConnectionAsync();
            
            _isInitialized = true;
            _logger.LogInformation("MCP Service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP Service");
            await CleanupAsync();
            throw;
        }
    }

    /// <summary>
    /// Get the list of available tools from the MCP server
    /// </summary>
    /// <returns>List of available tool names</returns>
    public async Task<List<string>> GetAvailableToolsAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting available tools from MCP server...");

            if (_mcpWriter == null || _mcpReader == null)
            {
                throw new InvalidOperationException("MCP connection is not established");
            }

            // Send list_tools request
            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/list",
                @params = new { }
            };

            
            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();

            _logger.LogDebug("Sent tools/list request: {Request}", requestJson);

            // Read response
            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                throw new InvalidOperationException("No response received from MCP server");
            }

            _logger.LogDebug("Received response: {Response}", responseJson);

            // Parse response
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            var toolNames = new List<string>();
            if (root.TryGetProperty("result", out var result) && 
                result.TryGetProperty("tools", out var tools))
            {
                foreach (var tool in tools.EnumerateArray())
                {
                    if (tool.TryGetProperty("name", out var nameProperty))
                    {
                        var name = nameProperty.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            toolNames.Add(name);
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {ToolCount} tools from MCP server", toolNames.Count);
            
            // Log tool categories for better understanding
            LogToolCategories(toolNames);

            return toolNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available tools from MCP server");
            throw new InvalidOperationException("Unable to retrieve tools from MCP server", ex);
        }
    }

    /// <summary>
    /// Get health status of the service including MCP connection
    /// </summary>
    public async Task<ServiceHealthResponse> GetHealthStatusAsync()
    {
        var health = new ServiceHealthResponse
        {
            LastChecked = DateTime.UtcNow
        };

        try
        {
            // Check MCP server connection
            health.McpServer.Connected = _isInitialized && 
                                       _mcpServerProcess != null && 
                                       !_mcpServerProcess.HasExited &&
                                       _mcpWriter != null && 
                                       _mcpReader != null;
            
            health.McpServer.Status = health.McpServer.Connected ? "Connected" : "Disconnected";
            health.McpServer.LastConnected = health.McpServer.Connected ? DateTime.UtcNow : null;

            // If MCP is connected, get available tools count
            if (health.McpServer.Connected)
            {
                try
                {
                    var tools = await GetAvailableToolsAsync();
                    health.AvailableToolsCount = tools.Count;
                    health.AvailableTools = tools;
                }
                catch (Exception ex)
                {
                    health.McpServer.ErrorMessage = ex.Message;
                    health.McpServer.Connected = false;
                    health.McpServer.Status = "Error";
                }
            }

            // Check Azure OpenAI configuration and connectivity
            try
            {
                var endpoint = _configuration["AzureOpenAI:Endpoint"];
                var apiKey = _configuration["AzureOpenAI:Key"];
                
                if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
                {
                    health.OpenAI.Connected = true;
                    health.OpenAI.Status = "Configured";
                    health.OpenAI.LastConnected = DateTime.UtcNow;
                }
                else
                {
                    health.OpenAI.Connected = false;
                    health.OpenAI.Status = "Not Configured";
                    health.OpenAI.ErrorMessage = "Azure OpenAI endpoint or API key not configured";
                }
            }
            catch (Exception ex)
            {
                health.OpenAI.Connected = false;
                health.OpenAI.Status = "Error";
                health.OpenAI.ErrorMessage = ex.Message;
            }

            health.IsHealthy = health.McpServer.Connected;
            health.Status = health.IsHealthy ? "Healthy" : "Unhealthy";

            if (!health.IsHealthy)
            {
                health.ErrorMessage = health.McpServer.ErrorMessage ?? "MCP server connection failed";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking service health");
            health.IsHealthy = false;
            health.Status = "Error";
            health.ErrorMessage = ex.Message;
        }

        return health;
    }

    /// <summary>
    /// Get detailed information about available tools including their parameters
    /// </summary>
    /// <returns>Dictionary of tool names with their parameter schemas</returns>
    public async Task<Dictionary<string, object>> GetToolsWithParametersAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting detailed tool information from MCP server...");

            if (_mcpWriter == null || _mcpReader == null)
            {
                throw new InvalidOperationException("MCP connection is not established");
            }

            // Send list_tools request
            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/list",
                @params = new { }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();

            _logger.LogDebug("Sent tools/list request: {Request}", requestJson);

            // Read response
            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                throw new InvalidOperationException("No response received from MCP server");
            }

            _logger.LogDebug("Received response: {Response}", responseJson);

            // Parse response
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            var toolsWithParameters = new Dictionary<string, object>();
            
            if (root.TryGetProperty("result", out var result) && 
                result.TryGetProperty("tools", out var tools))
            {
                foreach (var tool in tools.EnumerateArray())
                {
                    var toolInfo = new Dictionary<string, object>();
                    
                    // Get tool name
                    if (tool.TryGetProperty("name", out var nameProperty))
                    {
                        var name = nameProperty.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            // Get description
                            if (tool.TryGetProperty("description", out var descProperty))
                            {
                                toolInfo["description"] = descProperty.GetString() ?? "";
                            }

                            // Get input schema (parameters)
                            if (tool.TryGetProperty("inputSchema", out var schemaProperty))
                            {
                                toolInfo["inputSchema"] = JsonSerializer.Deserialize<object>(schemaProperty.GetRawText());
                            }

                            toolsWithParameters[name] = toolInfo;
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {ToolCount} tools with parameter information", toolsWithParameters.Count);
            
            return toolsWithParameters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tools with parameters from MCP server");
            throw new InvalidOperationException("Unable to retrieve tool parameters from MCP server", ex);
        }
    }

    /// <summary>
    /// Get the parameter schema for a specific tool
    /// </summary>
    /// <param name="toolName">Name of the tool</param>
    /// <returns>Tool parameter schema and description</returns>
    public async Task<object?> GetToolParametersAsync(string toolName)
    {
        var allTools = await GetToolsWithParametersAsync();
        
        if (allTools.TryGetValue(toolName, out var toolInfo))
        {
            return toolInfo;
        }

        throw new ArgumentException($"Tool '{toolName}' not found", nameof(toolName));
    }

    /// <summary>
    /// Execute a specific educational tool with parameters
    /// </summary>
    /// <param name="toolName">Name of the tool to execute</param>
    /// <param name="arguments">Arguments/parameters for the tool</param>
    /// <returns>Tool execution result</returns>
    public async Task<object> CallToolAsync(string toolName, object? arguments = null)
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Calling tool: {ToolName} with arguments: {Arguments}", 
                toolName, JsonSerializer.Serialize(arguments ?? new object()));

            if (_mcpWriter == null || _mcpReader == null)
            {
                throw new InvalidOperationException("MCP connection is not established");
            }

            // Send tools/call request
            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/call",
                @params = new
                {
                    name = toolName,
                    arguments = arguments ?? new object()
                }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();

            _logger.LogDebug("Sent tools/call request: {Request}", requestJson);

            // Read response
            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                throw new InvalidOperationException("No response received from MCP server");
            }

            _logger.LogDebug("Received tool call response: {Response}", responseJson);

            // Parse response
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            if (root.TryGetProperty("result", out var result))
            {
                return JsonSerializer.Deserialize<object>(result.GetRawText()) ?? new object();
            }

            throw new InvalidOperationException("Invalid response format from MCP server");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call tool: {ToolName}", toolName);
            throw new InvalidOperationException($"Unable to call tool '{toolName}'", ex);
        }
    }

    /// <summary>
    /// Get available resources from the MCP server
    /// </summary>
    /// <returns>List of available resources</returns>
    public async Task<List<object>> GetAvailableResourcesAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting available resources from MCP server...");

            if (_mcpWriter == null || _mcpReader == null)
            {
                throw new InvalidOperationException("MCP connection is not established");
            }

            // Send resources/list request
            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "resources/list",
                @params = new { }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();

            _logger.LogDebug("Sent resources/list request: {Request}", requestJson);

            // Read response
            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                throw new InvalidOperationException("No response received from MCP server");
            }

            _logger.LogDebug("Received resources response: {Response}", responseJson);

            // Parse response
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                _logger.LogWarning("MCP server returned error for resources/list: {Error}", errorMessage);
                return new List<object>(); // Resources might not be supported
            }

            var resources = new List<object>();
            if (root.TryGetProperty("result", out var result) && 
                result.TryGetProperty("resources", out var resourcesArray))
            {
                foreach (var resource in resourcesArray.EnumerateArray())
                {
                    var resourceObj = JsonSerializer.Deserialize<object>(resource.GetRawText());
                    if (resourceObj != null)
                    {
                        resources.Add(resourceObj);
                    }
                }
            }

            _logger.LogInformation("Retrieved {ResourceCount} resources from MCP server", resources.Count);
            
            return resources;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get resources from MCP server (might not be supported)");
            return new List<object>(); // Return empty list if resources not supported
        }
    }

    /// <summary>
    /// Process a request using Azure OpenAI with MCP tools integration
    /// </summary>
    /// <param name="userMessage">User's request message</param>
    /// <param name="systemPrompt">Optional system prompt for context</param>
    /// <returns>AI response after potentially using MCP tools</returns>
    public async Task<string> ProcessRequestWithAzureOpenAIAsync(string userMessage, string? systemPrompt = null)
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Processing request with Azure OpenAI: {Message}", userMessage);

            // Get Azure OpenAI configuration
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:Key"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4";

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Azure OpenAI configuration is missing. Endpoint: {EndpointProvided}, ApiKey: {ApiKeyProvided}", 
                    !string.IsNullOrEmpty(endpoint), !string.IsNullOrEmpty(apiKey));
                throw new InvalidOperationException("Azure OpenAI configuration is missing. Please configure Endpoint and ApiKey in the application settings.");
            }

            // Create Azure OpenAI client
            var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            var chatClient = client.GetChatClient(deploymentName);

            // Get available MCP tools and convert to OpenAI format
            var mcpTools = await GetToolsWithParametersAsync();
            var openAITools = ConvertMcpToolsToOpenAIFormat(mcpTools);

            _logger.LogInformation("Converted {ToolCount} MCP tools for OpenAI", openAITools.Count);

            // Build chat messages
            var messages = new List<ChatMessage>();

            // Add system prompt if provided
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new SystemChatMessage(systemPrompt));
            }
            else
            {
                // Default system prompt for educational context with tool usage encouragement
                messages.Add(new SystemChatMessage(
                    "You are an educational AI assistant with access to learning management tools. " +
                    "You have access to various educational management functions that you SHOULD use when appropriate. " +
                    "When users ask about courses, lessons, quizzes, categories, or answers, use the available tools to help them. " +
                    "Always try to use tools when the user's request relates to educational content management."));
            }

            messages.Add(new UserChatMessage(userMessage));

            // Create chat completion options with tools and explicit tool choice
            var options = new ChatCompletionOptions();
            
            // Add all tools to options
            foreach (var tool in openAITools)
            {
                options.Tools.Add(tool);
            }

            // Force tool usage for certain keywords
            if (ShouldForceToolUsage(userMessage))
            {
                options.ToolChoice = ChatToolChoice.CreateRequiredChoice();
                _logger.LogInformation("Forcing tool usage for request containing educational keywords");
            }
            else
            {
                options.ToolChoice = ChatToolChoice.CreateAutoChoice();
            }

            // Set temperature for more consistent responses
            options.Temperature = 0.1f;

            var maxIterations = 5; // Prevent infinite loops
            var currentIteration = 0;

            while (currentIteration < maxIterations)
            {
                currentIteration++;
                _logger.LogDebug("Azure OpenAI iteration {Iteration}", currentIteration);

                // Get response from Azure OpenAI
                var response = await chatClient.CompleteChatAsync(messages, options);
                var responseMessage = response.Value.Content.FirstOrDefault();

                // Log the response for debugging
                _logger.LogDebug("Received response with {ContentCount} content items", response.Value.Content.Count);

                // Check if the model wants to use tools
                var toolCalls = response.Value.ToolCalls;
                _logger.LogDebug("Tool calls count: {ToolCallCount}", toolCalls?.Count ?? 0);

                if (toolCalls?.Count > 0)
                {
                    _logger.LogInformation("Azure OpenAI requested {ToolCallCount} tool calls", toolCalls.Count);

                    // Add assistant's message with tool calls (content might be empty when using tools)
                    var assistantMessage = new AssistantChatMessage(responseMessage?.Text ?? "");
                    foreach (var toolCall in toolCalls)
                    {
                        if (toolCall is ChatToolCall functionCall)
                        {
                            assistantMessage.ToolCalls.Add(functionCall);
                        }
                    }
                    messages.Add(assistantMessage);

                    // Execute each tool call
                    foreach (var toolCall in toolCalls)
                    {
                        if (toolCall is ChatToolCall functionCall)
                        {
                            try
                            {
                                _logger.LogInformation("Executing tool: {ToolName} with arguments: {Arguments}",
                                    functionCall.FunctionName, functionCall.FunctionArguments);

                                // Parse arguments
                                object? arguments = null;
                                if (!string.IsNullOrEmpty(functionCall.FunctionArguments?.ToString()))
                                {
                                    try
                                    {
                                        arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(functionCall.FunctionArguments.ToString());
                                    }
                                    catch (JsonException ex)
                                    {
                                        _logger.LogWarning(ex, "Failed to parse tool arguments as dictionary, using raw object");
                                        arguments = JsonSerializer.Deserialize<object>(functionCall.FunctionArguments.ToString());
                                    }
                                }

                                // Call MCP tool
                                var toolResult = await CallToolAsync(functionCall.FunctionName, arguments);
                                var toolResultJson = JsonSerializer.Serialize(toolResult, new JsonSerializerOptions { WriteIndented = false });

                                // Add tool result to conversation
                                messages.Add(new ToolChatMessage(functionCall.Id, toolResultJson));

                                _logger.LogInformation("Tool {ToolName} executed successfully with result length: {ResultLength}", 
                                    functionCall.FunctionName, toolResultJson.Length);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error executing tool: {ToolName}", functionCall.FunctionName);

                                // Add error message to conversation
                                var errorMessage = $"Error executing tool {functionCall.FunctionName}: {ex.Message}";
                                messages.Add(new ToolChatMessage(functionCall.Id, errorMessage));
                            }
                        }
                    }

                    // Reset tool choice for subsequent calls
                    options.ToolChoice = ChatToolChoice.CreateAutoChoice();

                    // Continue the conversation to get the final response
                    continue;
                }

                // No tool calls - check if we have content to return
                if (responseMessage?.Text != null)
                {
                    return responseMessage.Text;
                }

                // If no content and no tool calls, this might be an error condition
                _logger.LogWarning("Received response with no content and no tool calls");
                return "Je n'ai pas pu générer une réponse appropriée. Veuillez reformuler votre demande.";
            }

            _logger.LogWarning("Maximum iterations reached in Azure OpenAI conversation");
            return "Je n'ai pas pu terminer le traitement de votre demande dans le temps imparti. Veuillez réessayer.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process request with Azure OpenAI");
            return "Je rencontre actuellement des difficultés techniques. Veuillez réessayer plus tard.";
        }
    }

    /// <summary>
    /// Determine if tool usage should be forced based on user message content
    /// </summary>
    /// <param name="userMessage">User's message</param>
    /// <returns>True if tools should be required</returns>
    private bool ShouldForceToolUsage(string userMessage)
    {
        var lowerMessage = userMessage.ToLowerInvariant();
        var educationalKeywords = new[]
        {
            "course", "cours", "lesson", "leçon", "quiz", "test", "category", "catégorie",
            "create", "créer", "update", "modifier", "delete", "supprimer", "list", "lister",
            "answer", "réponse", "student", "étudiant", "instructor", "professeur"
        };

        return educationalKeywords.Any(keyword => lowerMessage.Contains(keyword));
    }

    /// <summary>
    /// Convert MCP tools to OpenAI function calling format with improved error handling
    /// </summary>
    /// <param name="mcpTools">MCP tools with their schemas</param>
    /// <returns>List of OpenAI chat tools</returns>
    private List<ChatTool> ConvertMcpToolsToOpenAIFormat(Dictionary<string, object> mcpTools)
    {
        var openAITools = new List<ChatTool>();

        foreach (var (toolName, toolInfo) in mcpTools)
        {
            try
            {
                if (toolInfo is Dictionary<string, object> info)
                {
                    var description = info.GetValueOrDefault("description", $"Educational management tool: {toolName}").ToString() ?? $"Tool: {toolName}";

                    // Get input schema with fallback
                    object? parametersObj = info.GetValueOrDefault("inputSchema", null);
                    
                    // Create a default schema if none provided
                    if (parametersObj == null)
                    {
                        parametersObj = new
                        {
                            type = "object",
                            properties = new { },
                            additionalProperties = true
                        };
                    }

                    var parametersJson = JsonSerializer.Serialize(parametersObj);
                    var parametersBinary = BinaryData.FromString(parametersJson);

                    // Create the OpenAI function tool
                    var chatTool = ChatTool.CreateFunctionTool(toolName, description, parametersBinary);
                    openAITools.Add(chatTool);

                    _logger.LogDebug("Converted MCP tool {ToolName} to OpenAI format with description: {Description}", 
                        toolName, description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to convert MCP tool {ToolName} to OpenAI format, creating basic tool", toolName);
                
                // Create a basic tool as fallback
                try
                {
                    var basicSchema = new
                    {
                        type = "object",
                        properties = new { },
                        additionalProperties = true
                    };
                    var basicParametersJson = JsonSerializer.Serialize(basicSchema);
                    var basicParametersBinary = BinaryData.FromString(basicParametersJson);
                    
                    var basicTool = ChatTool.CreateFunctionTool(
                        toolName, 
                        $"Educational management tool: {toolName}", 
                        basicParametersBinary
                    );
                    openAITools.Add(basicTool);
                    
                    _logger.LogInformation("Created basic fallback tool for {ToolName}", toolName);
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Failed to create fallback tool for {ToolName}", toolName);
                }
            }
        }

        _logger.LogInformation("Successfully converted {Count} MCP tools to OpenAI format", openAITools.Count);
        return openAITools;
    }

    /// <summary>
    /// Process educational request with intelligent tool usage and Microsoft Learn integration
    /// </summary>
    /// <param name="request">User's educational request</param>
    /// <returns>Comprehensive response using appropriate tools and Microsoft Learn knowledge</returns>
    public async Task<string> ProcessIntelligentEducationalRequestAsync(string request)
    {
        var systemPrompt = @"
        You are an expert educational AI assistant specializing in learning management and educational technology, powered by Microsoft Learn knowledge and educational management tools.

        ## Your Capabilities:
        You have access to comprehensive educational management tools for:
        - **Course Management**: Create, modify, and manage educational courses
        - **Lesson Content**: Develop and organize pedagogical content and learning materials
        - **Assessment Tools**: Create and manage quizzes, evaluations, and learning assessments
        - **Content Organization**: Structure and categorize educational content effectively
        - **Performance Analytics**: Analyze student responses, progress, and learning outcomes

        ## Microsoft Learn Integration:
        You can access official Microsoft documentation and best practices for:
        - Azure AI services for education
        - Learning management system architecture
        - Educational technology implementation
        - AI-powered learning solutions
        - Modern educational platform development

        ## Response Guidelines:
        1. **Use Tools Intelligently**: Leverage the available MCP tools when users request specific educational management tasks
        2. **Reference Best Practices**: Draw from Microsoft Learn documentation for technical implementation guidance
        3. **Provide Comprehensive Solutions**: Combine tool usage with educational expertise for complete answers
        4. **Maintain Educational Focus**: Always prioritize pedagogical effectiveness and learning outcomes
        5. **Be Actionable**: Provide specific, implementable solutions rather than general advice

        ## Communication Style:
        - Respond in the user's preferred language (default to English unless specified otherwise)
        - Provide detailed, practical guidance
        - Include step-by-step instructions when appropriate
        - Reference relevant Microsoft Learn resources when applicable
        - Explain the educational rationale behind technical recommendations

        When users ask about educational technology implementation, architecture, or best practices, search Microsoft Learn documentation first to provide authoritative, up-to-date guidance before using the educational management tools.
        ";

        return await ProcessRequestWithAzureOpenAIAsync(request, systemPrompt);
    }

    #region Private Methods

    private async Task StartMcpServerProcessAsync()
    {
        try
        {
            _logger.LogInformation("Starting MCP server process at: {Path}", _mcpServerPath);

            // Validate the directory exists
            if (!Directory.Exists(_mcpServerPath))
            {
                throw new DirectoryNotFoundException($"MCP server directory not found: {_mcpServerPath}");
            }

            // Validate the project file exists
            var projectFile = Path.Combine(_mcpServerPath, "learnify.ai.mcp.server.csproj");
            if (!File.Exists(projectFile))
            {
                throw new FileNotFoundException($"MCP server project file not found: {projectFile}");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = _mcpServerExecutable,
                Arguments = "run --configuration Release --no-build --verbosity quiet --nologo",
                WorkingDirectory = _mcpServerPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                Environment =
                {
                    ["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1",
                    ["DOTNET_NOLOGO"] = "1",
                    ["ASPNETCORE_ENVIRONMENT"] = "Development",
                    ["SUPPRESS_STARTUP_MESSAGE"] = "true"
                }
            };

            _mcpServerProcess = new Process { StartInfo = startInfo };
            
            // Handle process exit
            _mcpServerProcess.EnableRaisingEvents = true;
            _mcpServerProcess.Exited += (sender, e) =>
            {
                _logger.LogWarning("MCP server process exited unexpectedly with code: {ExitCode}", 
                    _mcpServerProcess?.ExitCode);
            };

            // Handle stderr for logging (MCP protocol uses stderr for logs)
            _mcpServerProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    _logger.LogDebug("MCP Server: {Message}", e.Data);
                }
            };

            if (!_mcpServerProcess.Start())
            {
                throw new InvalidOperationException("Failed to start MCP server process");
            }

            // Start async stderr reading
            _mcpServerProcess.BeginErrorReadLine();

            // Set up streams for communication
            _mcpWriter = new StreamWriter(_mcpServerProcess.StandardInput.BaseStream, System.Text.Encoding.UTF8)
            {
                AutoFlush = true
            };
            _mcpReader = new StreamReader(_mcpServerProcess.StandardOutput.BaseStream, System.Text.Encoding.UTF8);

            // Give the server more time to start up
            await Task.Delay(3000);

            if (_mcpServerProcess.HasExited)
            {
                var exitCode = _mcpServerProcess.ExitCode;
                throw new InvalidOperationException(
                    $"MCP server process exited immediately with code: {exitCode}. " +
                    $"Check that the server project is built and all dependencies are available.");
            }

            _logger.LogInformation("MCP server process started successfully (PID: {ProcessId})", _mcpServerProcess.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MCP server process. Working directory: {WorkingDirectory}, Executable: {Executable}", 
                _mcpServerPath, _mcpServerExecutable);
            throw;
        }
    }

    private async Task InitializeMcpConnectionAsync()
    {
        try
        {
            if (_mcpWriter == null || _mcpReader == null)
            {
                throw new InvalidOperationException("MCP streams are not available");
            }

            _logger.LogInformation("Initializing MCP connection...");

            // Send initialize request
            var initRequest = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "initialize",
                @params = new
                {
                    clientInfo = new
                    {
                        name = "learnify-ai-client",
                        version = "1.0.0"
                    },
                    protocolVersion = "2024-11-05"
                }
            };

            var requestJson = JsonSerializer.Serialize(initRequest);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();

            _logger.LogDebug("Sent initialize request: {Request}", requestJson);

            // Read response
            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                throw new InvalidOperationException("No initialization response received from MCP server");
            }

            _logger.LogDebug("Received initialize response: {Response}", responseJson);

            // Parse response
            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server initialization failed: {errorMessage}");
            }

            // Send initialized notification
            var initializedNotification = new
            {
                jsonrpc = "2.0",
                method = "notifications/initialized",
                @params = new { }
            };

            var notificationJson = JsonSerializer.Serialize(initializedNotification);
            await _mcpWriter.WriteLineAsync(notificationJson);
            await _mcpWriter.FlushAsync();

            _logger.LogInformation("MCP connection initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP connection");
            throw;
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }

    private void LogToolCategories(List<string> tools)
    {
        var toolsByCategory = GroupToolsByCategory(tools);
        
        _logger.LogInformation("Available tools by category:");
        foreach (var category in toolsByCategory)
        {
            _logger.LogInformation("  {Category}: {Count} tools", category.Key, category.Value.Count);
        }
    }

    private static Dictionary<string, List<string>> GroupToolsByCategory(List<string> tools)
    {
        return tools.GroupBy(tool =>
        {
            var lowerTool = tool.ToLowerInvariant();
            if (lowerTool.Contains("lesson")) return "Lessons";
            if (lowerTool.Contains("course")) return "Courses";
            if (lowerTool.Contains("category")) return "Categories";
            if (lowerTool.Contains("quiz")) return "Quizzes";
            if (lowerTool.Contains("answer")) return "Answers";
            return "Other";
        }).ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task CleanupAsync()
    {
        try
        {
            _mcpWriter?.Dispose();
            _mcpWriter = null;

            _mcpReader?.Dispose();
            _mcpReader = null;

            if (_mcpServerProcess != null && !_mcpServerProcess.HasExited)
            {
                _mcpServerProcess.Kill();
                await _mcpServerProcess.WaitForExitAsync();
            }

            _mcpServerProcess?.Dispose();
            _mcpServerProcess = null;
            _isInitialized = false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during cleanup");
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            CleanupAsync().Wait();
            _disposed = true;
        }
    }

    #endregion
}
