using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// AI Agent service that orchestrates Azure OpenAI with MCP tool schemas and executions
/// </summary>
public class AiAgentService
{
    private readonly ILogger<AiAgentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly McpToolsService _toolsService;

    public AiAgentService(
        ILogger<AiAgentService> logger,
        IConfiguration configuration,
        McpToolsService toolsService)
    {
        _logger = logger;
        _configuration = configuration;
        _toolsService = toolsService;
    }

    /// <summary>
    /// Process a request using Azure OpenAI with MCP tools integration
    /// </summary>
    public async Task<string> ProcessRequestWithAzureOpenAIAsync(string userMessage, string? systemPrompt = null)
    {
        // Ensure MCP is up because we'll need tool schemas and possibly execute tools
        await _toolsService.EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Processing request with Azure OpenAI (AI Agent): {Message}", userMessage);

            // Get Azure OpenAI configuration
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:Key"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4";

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Azure OpenAI configuration is missing. Endpoint configured: {EndpointConfigured}, ApiKey configured: {KeyConfigured}",
                    !string.IsNullOrEmpty(endpoint), !string.IsNullOrEmpty(apiKey));
                throw new InvalidOperationException("Azure OpenAI configuration is missing. Please configure Endpoint and ApiKey in the application settings.");
            }

            // Create Azure OpenAI client
            var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            var chatClient = client.GetChatClient(deploymentName);

            // Get available MCP tools and convert to OpenAI format
            var mcpTools = await _toolsService.GetToolsWithParametersAsync();
            var openAITools = ConvertMcpToolsToOpenAIFormat(mcpTools);

            _logger.LogInformation("Converted {ToolCount} MCP tools for OpenAI", openAITools.Count);

            // Build chat messages
            var messages = new List<ChatMessage>();

            // Add system prompt if provided
            messages.Add(!string.IsNullOrEmpty(systemPrompt)
                ? new SystemChatMessage(systemPrompt)
                : new SystemChatMessage(
                    "You are an educational AI assistant with access to learning management tools. " +
                    "Use the available tools when the user's request relates to courses, lessons, quizzes, categories, or answers."));

            messages.Add(new UserChatMessage(userMessage));

            // Create chat completion options with tools and explicit tool choice
            var options = new ChatCompletionOptions();
            foreach (var tool in openAITools)
                options.Tools.Add(tool);

            options.ToolChoice = ShouldForceToolUsage(userMessage)
                ? ChatToolChoice.CreateRequiredChoice()
                : ChatToolChoice.CreateAutoChoice();

            options.Temperature = 0.1f;

            var maxIterations = 10; // Prevent infinite loops
            for (var currentIteration = 0; currentIteration < maxIterations; currentIteration++)
            {
                _logger.LogDebug("Azure OpenAI iteration {Iteration}", currentIteration + 1);

                // Get response from Azure OpenAI
                var response = await chatClient.CompleteChatAsync(messages, options);
                var responseMessage = response.Value.Content.FirstOrDefault();

                _logger.LogDebug("Received response with {ContentCount} content items", response.Value.Content.Count);

                // Check tool calls first (content can be null here)
                var toolCalls = response.Value.ToolCalls;
                _logger.LogDebug("Tool calls count: {ToolCallCount}", toolCalls?.Count ?? 0);
                if (toolCalls?.Count > 0)
                {
                    _logger.LogInformation("Azure OpenAI requested {ToolCallCount} tool calls", toolCalls.Count);

                    // Add assistant's message with tool calls
                    var assistantMessage = new AssistantChatMessage(responseMessage?.Text ?? string.Empty);
                    foreach (var toolCall in toolCalls.OfType<ChatToolCall>())
                        assistantMessage.ToolCalls.Add(toolCall);
                    messages.Add(assistantMessage);

                    // Execute each tool call
                    foreach (var functionCall in toolCalls.OfType<ChatToolCall>())
                    {
                        try
                        {
                            _logger.LogInformation("Executing tool: {ToolName} with arguments: {Arguments}",
                                functionCall.FunctionName, functionCall.FunctionArguments);

                            // Parse arguments
                            object? arguments = null;
                            var rawArgs = functionCall.FunctionArguments?.ToString();
                            if (!string.IsNullOrWhiteSpace(rawArgs))
                            {
                                try
                                {
                                    arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(rawArgs!);
                                }
                                catch (JsonException ex)
                                {
                                    _logger.LogWarning(ex, "Failed to parse tool arguments as dictionary, using raw object");
                                    arguments = JsonSerializer.Deserialize<object>(rawArgs!);
                                }
                            }

                            // Call MCP tool via tools service
                            var toolResult = await _toolsService.CallToolAsync(functionCall.FunctionName, arguments);
                            var toolResultJson = JsonSerializer.Serialize(toolResult);

                            // Add tool result to conversation
                            messages.Add(new ToolChatMessage(functionCall.Id, toolResultJson));

                            _logger.LogInformation("Tool {ToolName} executed successfully with result length: {ResultLength}",
                                functionCall.FunctionName, toolResultJson.Length);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing tool: {ToolName}", functionCall.FunctionName);
                            var errorMessage = $"Error executing tool {functionCall.FunctionName}: {ex.Message}";
                            messages.Add(new ToolChatMessage(functionCall.Id, errorMessage));
                        }
                    }

                    // Reset tool choice for subsequent calls
                    options.ToolChoice = ChatToolChoice.CreateAutoChoice();

                    // Continue conversation
                    continue;
                }

                // No tool calls - return content if any
                if (!string.IsNullOrEmpty(responseMessage?.Text))
                    return responseMessage!.Text;

                // If no content and no tool calls, warn and return a helpful message
                _logger.LogWarning("Received response with no content and no tool calls");
                return "I couldn't generate an appropriate response. Please try rephrasing your request.";
            }

            _logger.LogWarning("Maximum iterations reached in Azure OpenAI conversation");
            return "I couldn't complete your request in time. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process request with Azure OpenAI (AI Agent)");
            return "I'm experiencing technical difficulties. Please try again later.";
        }
    }

    /// <summary>
    /// Process educational request with intelligent tool usage and Microsoft Learn integration
    /// </summary>
    public Task<string> ProcessIntelligentEducationalRequestAsync(string request)
    {
        var systemPrompt = @"
            You are an expert educational AI assistant specializing in learning management and educational technology, powered by Microsoft Learn knowledge and educational management tools.

            - Use MCP tools for course, lesson, quiz, category, and analytics operations.
            - Provide actionable, pedagogy-focused guidance with best practices.
            - Reference Microsoft Learn documentation when appropriate.
            ";
        return ProcessRequestWithAzureOpenAIAsync(request, systemPrompt);
    }

    private static bool ShouldForceToolUsage(string userMessage)
    {
        var lowerMessage = userMessage.ToLowerInvariant();
        var educationalKeywords = new[]
        {
            "course", "cours", "lesson", "leçon", "quiz", "test", "category", "catégorie",
            "create", "créer", "update", "modifier", "delete", "supprimer", "list", "lister",
            "answer", "réponse", "student", "étudiant", "instructor", "professeur"
        };
        return educationalKeywords.Any(lowerMessage.Contains);
    }

    private static List<ChatTool> ConvertMcpToolsToOpenAIFormat(Dictionary<string, object> mcpTools)
    {
        var openAITools = new List<ChatTool>();

        foreach (var (toolName, toolInfo) in mcpTools)
        {
            try
            {
                if (toolInfo is Dictionary<string, object> info)
                {
                    var description = info.GetValueOrDefault("description", $"Educational management tool: {toolName}")?.ToString() ?? $"Tool: {toolName}";

                    object? parametersObj = info.GetValueOrDefault("inputSchema", null);
                    parametersObj ??= new { type = "object", properties = new { }, additionalProperties = true };

                    var parametersJson = JsonSerializer.Serialize(parametersObj);
                    var parametersBinary = BinaryData.FromString(parametersJson);

                    openAITools.Add(ChatTool.CreateFunctionTool(toolName, description, parametersBinary));
                }
            }
            catch
            {
                var basicSchema = new { type = "object", properties = new { }, additionalProperties = true };
                openAITools.Add(ChatTool.CreateFunctionTool(toolName, $"Educational management tool: {toolName}", BinaryData.FromObjectAsJson(basicSchema)));
            }
        }

        return openAITools;
    }
}
