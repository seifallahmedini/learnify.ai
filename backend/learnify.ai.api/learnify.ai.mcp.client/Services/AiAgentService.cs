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

            // Enhanced system prompt for better error handling and tool retry logic
            var enhancedSystemPrompt = !string.IsNullOrEmpty(systemPrompt) ? systemPrompt : 
                "You are an educational AI assistant with access to learning management tools.";

            enhancedSystemPrompt += @"
CRITICAL TOOL USAGE GUIDELINES - FOLLOW THESE RULES:
1. When a tool call fails with validation errors, ANALYZE the error message carefully
2. IMMEDIATELY RETRY the tool call with corrected parameters based on the error feedback
3. For missing required fields, either:
   - Use reasonable defaults based on the context
   - Generate appropriate content (like learning objectives, descriptions, etc.)
   - NEVER ask the user for missing information if you can reasonably infer it
4. For validation errors (like negative prices, invalid levels), correct the values automatically
5. ALWAYS attempt to complete the user's request rather than just explaining what went wrong
6. When creating courses, lessons, or other educational content:
   - Generate appropriate learning objectives if missing
   - Use reasonable defaults for optional fields
   - Infer missing information from the context when possible

MANDATORY BEHAVIOR:
- If a tool fails, you MUST call it again with fixes
- Do NOT respond with text explanations about errors
- Do NOT ask users for information you can generate
- Your job is to COMPLETE tasks, not explain why they can't be done

Examples:
- If 'learningObjectives' is required but missing, generate relevant objectives based on the course title and description
- If 'shortDescription' is missing, create one from the main description
- If price is negative, use 0 or a reasonable positive value
- If level is invalid, choose an appropriate level (1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert)

Be proactive and helpful - don't just report errors, FIX them and retry!";

            messages.Add(new SystemChatMessage(enhancedSystemPrompt));
            messages.Add(new UserChatMessage(userMessage));

            // Create chat completion options with tools and explicit tool choice
            var options = new ChatCompletionOptions();
            foreach (var tool in openAITools)
                options.Tools.Add(tool);

            options.ToolChoice = ShouldForceToolUsage(userMessage)
                ? ChatToolChoice.CreateRequiredChoice()
                : ChatToolChoice.CreateAutoChoice();

            options.Temperature = 0.7f; // Slightly higher for more creative problem-solving

            var maxIterations = 25; // Increased to allow for retries
            var consecutiveFailures = 0;
            var maxConsecutiveFailures = 3;

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

                    var hasAnyToolFailure = false;

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

                            // Analyze tool result for success/failure
                            var isToolSuccess = AnalyzeToolResult(toolResult);
                            
                            if (!isToolSuccess)
                            {
                                hasAnyToolFailure = true;
                                _logger.LogWarning("Tool {ToolName} failed with result: {Result}", 
                                    functionCall.FunctionName, toolResultJson);
                                
                                // Add enhanced error context for the AI to understand and retry
                                var enhancedErrorResult = EnhanceErrorResultForAI(toolResult, functionCall.FunctionName, arguments);
                                var enhancedResultJson = JsonSerializer.Serialize(enhancedErrorResult);
                                
                                messages.Add(new ToolChatMessage(functionCall.Id, enhancedResultJson));
                            }
                            else
                            {
                                // Add successful tool result to conversation
                                messages.Add(new ToolChatMessage(functionCall.Id, toolResultJson));
                                _logger.LogInformation("Tool {ToolName} executed successfully", functionCall.FunctionName);
                            }
                        }
                        catch (Exception ex)
                        {
                            hasAnyToolFailure = true;
                            _logger.LogError(ex, "Error executing tool: {ToolName}", functionCall.FunctionName);
                            
                            var errorResult = new
                            {
                                success = false,
                                message = $"Tool execution failed: {ex.Message}",
                                errorType = "ToolExecutionError",
                                toolName = functionCall.FunctionName,
                                suggestion = "Please check the tool parameters and try again with corrected values."
                            };
                            
                            messages.Add(new ToolChatMessage(functionCall.Id, JsonSerializer.Serialize(errorResult)));
                        }
                    }

                    // Handle consecutive failures
                    if (hasAnyToolFailure)
                    {
                        consecutiveFailures++;
                        if (consecutiveFailures >= maxConsecutiveFailures)
                        {
                            // Add a message to encourage the AI to try a different approach or use tools more effectively
                            messages.Add(new SystemChatMessage(
                                "You have encountered multiple tool failures. IMPORTANT: Do not give up! Instead of explaining the errors to the user, you must:" +
                                "1) ANALYZE the error details carefully" +
                                "2) GENERATE the missing required information (like learningObjectives, shortDescription, etc.)" +
                                "3) CALL THE TOOL AGAIN with ALL required parameters filled in" +
                                "4) Only ask the user for help if you absolutely cannot infer the missing information" +
                                "Your goal is to COMPLETE the user's request, not explain why it failed. Use your creativity to fill in reasonable defaults."));
                            
                            // Reset consecutive failures to give more attempts
                            consecutiveFailures = 0;
                        }
                    }
                    else
                    {
                        consecutiveFailures = 0; // Reset on success
                    }

                    // Force tool choice for next iteration when we had failures
                    if (hasAnyToolFailure && consecutiveFailures < maxConsecutiveFailures)
                    {
                        options.ToolChoice = ChatToolChoice.CreateRequiredChoice();
                    }
                    else
                    {
                        options.ToolChoice = ChatToolChoice.CreateAutoChoice();
                    }

                    // Reset tool choice for subsequent calls to allow AI to decide
                    // options.ToolChoice = ChatToolChoice.CreateAutoChoice();

                    // Continue conversation to let AI analyze results and potentially retry
                    continue;
                }

                // No tool calls - return content if any
                if (!string.IsNullOrEmpty(responseMessage?.Text))
                {
                    // If we had recent failures, the AI might be providing instructions instead of retrying
                    if (consecutiveFailures > 0 && IsResponseJustExplainingError(responseMessage.Text))
                    {
                        _logger.LogInformation("AI provided explanation instead of retry, forcing tool usage");
                        messages.Add(new AssistantChatMessage(responseMessage.Text));
                        messages.Add(new SystemChatMessage(
                            "STOP explaining errors! You must use tools to complete the user's request. " +
                            "Call the appropriate tool again RIGHT NOW with corrected parameters. " +
                            "Generate any missing required information based on the context and user's request. " +
                            "DO NOT respond with text - use tools only!"));
                        
                        // Force tool usage for next iteration
                        options.ToolChoice = ChatToolChoice.CreateRequiredChoice();
                        continue;
                    }
                    
                    // Check if this looks like an explanation when we expect action
                    if (ShouldForceToolUsage(userMessage) && IsResponseJustExplainingError(responseMessage.Text))
                    {
                        _logger.LogInformation("AI explaining instead of using tools for action request, redirecting");
                        messages.Add(new AssistantChatMessage(responseMessage.Text));
                        messages.Add(new SystemChatMessage(
                            "The user's request requires using tools to take action. Stop explaining and use the appropriate tools to complete their request. " +
                            "Generate any missing information and call the tools now."));
                        
                        options.ToolChoice = ChatToolChoice.CreateRequiredChoice();
                        continue;
                    }
                    
                    return responseMessage.Text;
                }

                // If no content and no tool calls, warn and return a helpful message
                _logger.LogWarning("Received response with no content and no tool calls");
                return "I couldn't generate an appropriate response. Please try rephrasing your request.";
            }

            _logger.LogWarning("Maximum iterations reached in Azure OpenAI conversation");
            return "I couldn't complete your request in the allowed time. The request may be too complex or require additional information.";
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

    /// <summary>
    /// Analyze tool result to determine if it was successful
    /// </summary>
    private static bool AnalyzeToolResult(object toolResult)
    {
        try
        {
            var resultJson = JsonSerializer.Serialize(toolResult);
            using var document = JsonDocument.Parse(resultJson);
            var root = document.RootElement;

            // Check for success property
            if (root.TryGetProperty("success", out var successElement))
            {
                return successElement.GetBoolean();
            }

            // If no success property, assume success if no obvious error indicators
            return !root.TryGetProperty("error", out _) && 
                   !root.TryGetProperty("errorType", out _);
        }
        catch
        {
            // If we can't parse, assume success
            return true;
        }
    }

    /// <summary>
    /// Enhance error result with AI-friendly context and suggestions
    /// </summary>
    private static object EnhanceErrorResultForAI(object originalResult, string toolName, object? arguments)
    {
        try
        {
            var resultJson = JsonSerializer.Serialize(originalResult);
            using var document = JsonDocument.Parse(resultJson);
            var root = document.RootElement;

            var enhanced = new Dictionary<string, object>
            {
                ["originalResult"] = originalResult,
                ["toolName"] = toolName,
                ["attemptedArguments"] = arguments ?? new { },
            };

            // Extract error details and provide AI-friendly suggestions
            if (root.TryGetProperty("details", out var detailsElement))
            {
                var details = detailsElement.GetString() ?? "";
                enhanced["errorDetails"] = details;
                enhanced["suggestions"] = GenerateRetrySuccestions(details, toolName);
            }
            else if (root.TryGetProperty("message", out var messageElement))
            {
                var message = messageElement.GetString() ?? "";
                enhanced["errorMessage"] = message;
                enhanced["suggestions"] = GenerateRetrySuccestions(message, toolName);
            }

            enhanced["retryInstructions"] = GetRetryInstructions(toolName);

            return enhanced;
        }
        catch
        {
            return originalResult;
        }
    }

    /// <summary>
    /// Generate specific retry suggestions based on error details
    /// </summary>
    private static List<string> GenerateRetrySuccestions(string errorDetails, string toolName)
    {
        var suggestions = new List<string>();
        var lowerError = errorDetails.ToLowerInvariant();

        if (lowerError.Contains("learning objectives") && lowerError.Contains("required"))
        {
            suggestions.Add("Generate appropriate learning objectives based on the course title and description");
            suggestions.Add("Example: 'Understand key concepts, Apply practical skills, Analyze real-world scenarios'");
        }

        if (lowerError.Contains("short description") && lowerError.Contains("required"))
        {
            suggestions.Add("Create a brief summary from the main description (1-2 sentences)");
        }

        if (lowerError.Contains("title") && lowerError.Contains("required"))
        {
            suggestions.Add("Ensure a descriptive title is provided");
        }

        if (lowerError.Contains("description") && lowerError.Contains("required"))
        {
            suggestions.Add("Provide a detailed course description");
        }

        if (lowerError.Contains("price") && lowerError.Contains("negative"))
        {
            suggestions.Add("Use a positive price value or 0 for free courses");
        }

        if (lowerError.Contains("level") && lowerError.Contains("invalid"))
        {
            suggestions.Add("Use valid level: 1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert");
        }

        if (lowerError.Contains("instructor") && lowerError.Contains("not found"))
        {
            suggestions.Add("Check if the instructor ID exists or use a valid instructor ID");
        }

        if (lowerError.Contains("category") && lowerError.Contains("not found"))
        {
            suggestions.Add("Check if the category ID exists or use a valid category ID");
        }

        if (suggestions.Count == 0)
        {
            suggestions.Add("Review the error message and correct the invalid parameters");
            suggestions.Add("Ensure all required fields are provided with valid values");
        }

        return suggestions;
    }

    /// <summary>
    /// Get tool-specific retry instructions
    /// </summary>
    private static string GetRetryInstructions(string toolName)
    {
        return toolName.ToLowerInvariant() switch
        {
            var name when name.Contains("create") && name.Contains("course") =>
                "For course creation, ensure: title, description, instructorId, categoryId, price (≥0), durationHours, level (1-4), language. Generate learningObjectives if missing.",
            
            var name when name.Contains("update") && name.Contains("course") =>
                "For course updates, provide the courseId and any fields to update with valid values.",
            
            var name when name.Contains("create") && name.Contains("lesson") =>
                "For lesson creation, ensure: courseId, title, description, content. Duration and other fields are optional.",
                
            _ => "Check the tool parameters and ensure all required fields are provided with valid values."
        };
    }

    /// <summary>
    /// Check if the response is just explaining an error instead of taking action
    /// </summary>
    private static bool IsResponseJustExplainingError(string response)
    {
        var lowerResponse = response.ToLowerInvariant();
        var explanatoryPhrases = new[]
        {
            "failed because", "you need to", "please provide", "is required",
            "to proceed", "would you like to", "here are some example",
            "missing", "cannot be", "error", "problem", "issue"
        };

        var actionPhrases = new[]
        {
            "i've created", "successfully", "here is", "completed", "i'll",
            "i have created", "creation was successful", "i created", "done"
        };

        var hasExplanatoryPhrases = explanatoryPhrases.Any(lowerResponse.Contains);
        var hasActionPhrases = actionPhrases.Any(lowerResponse.Contains);

        // Consider it an explanation if it has explanatory phrases but no action phrases
        // OR if it's asking questions instead of providing results
        var isAskingQuestions = lowerResponse.Contains("would you like") || 
                               lowerResponse.Contains("do you want") || 
                               lowerResponse.Contains("please specify") ||
                               lowerResponse.Contains("?");

        return (hasExplanatoryPhrases && !hasActionPhrases) || isAskingQuestions;
    }
}
