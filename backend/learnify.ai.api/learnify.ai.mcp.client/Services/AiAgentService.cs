using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// AI Agent service that orchestrates Azure OpenAI with MCP tool schemas and executions
/// Follows SOLID principles with separated concerns
/// </summary>
public class AiAgentService
{
    private readonly ILogger<AiAgentService> _logger;
    private readonly McpToolsService _toolsService;
    private readonly IAzureOpenAIClientFactory _clientFactory;
    private readonly IToolConverter _toolConverter;
    private readonly IToolExecutor _toolExecutor;
    private readonly IToolResultAnalyzer _resultAnalyzer;
    private readonly IErrorEnhancer _errorEnhancer;
    private readonly IResponseAnalyzer _responseAnalyzer;
    private readonly IToolChoiceStrategy _toolChoiceStrategy;
    private readonly ISystemPromptBuilder _systemPromptBuilder;

    public AiAgentService(
        ILogger<AiAgentService> logger,
        McpToolsService toolsService,
        IAzureOpenAIClientFactory clientFactory,
        IToolConverter toolConverter,
        IToolExecutor toolExecutor,
        IToolResultAnalyzer resultAnalyzer,
        IErrorEnhancer errorEnhancer,
        IResponseAnalyzer responseAnalyzer,
        IToolChoiceStrategy toolChoiceStrategy,
        ISystemPromptBuilder systemPromptBuilder)
    {
        _logger = logger;
        _toolsService = toolsService;
        _clientFactory = clientFactory;
        _toolConverter = toolConverter;
        _toolExecutor = toolExecutor;
        _resultAnalyzer = resultAnalyzer;
        _errorEnhancer = errorEnhancer;
        _responseAnalyzer = responseAnalyzer;
        _toolChoiceStrategy = toolChoiceStrategy;
        _systemPromptBuilder = systemPromptBuilder;
    }

    /// <summary>
    /// Process a request using Azure OpenAI with MCP tools integration
    /// </summary>
    public async Task<string> ProcessRequestWithAzureOpenAIAsync(string userMessage, string? systemPrompt = null)
    {
        await _toolsService.EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Processing request with Azure OpenAI (AI Agent): {Message}", userMessage);

            var context = await InitializeConversationAsync(userMessage, systemPrompt);
            return await ProcessConversationAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process request with Azure OpenAI (AI Agent)");
            return ConversationConstants.TechnicalDifficultyMessage;
        }
    }

    /// <summary>
    /// Initializes the conversation context with client, tools, and messages
    /// </summary>
    private async Task<ConversationContext> InitializeConversationAsync(string userMessage, string? systemPrompt)
    {
        var chatClient = _clientFactory.CreateChatClient();
        var mcpTools = await _toolsService.GetToolsWithParametersAsync();
        var openAITools = _toolConverter.ConvertToOpenAIFormat(mcpTools);

        _logger.LogInformation("Converted {ToolCount} MCP tools for OpenAI", openAITools.Count);

        var enhancedSystemPrompt = _systemPromptBuilder.BuildSystemPrompt(systemPrompt);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(enhancedSystemPrompt),
            new UserChatMessage(userMessage)
        };

        var options = CreateChatCompletionOptions(openAITools, userMessage);

        return new ConversationContext
        {
            ChatClient = chatClient,
            Messages = messages,
            Options = options,
            UserMessage = userMessage
        };
    }

    /// <summary>
    /// Creates chat completion options with tools and tool choice strategy
    /// </summary>
    private ChatCompletionOptions CreateChatCompletionOptions(List<ChatTool> openAITools, string userMessage)
    {
        var options = new ChatCompletionOptions();
        foreach (var tool in openAITools)
            options.Tools.Add(tool);

        options.ToolChoice = _toolChoiceStrategy.ShouldForceToolUsage(userMessage)
            ? ChatToolChoice.CreateRequiredChoice()
            : ChatToolChoice.CreateAutoChoice();

        options.Temperature = ConversationConstants.DefaultTemperature;
        return options;
    }

    /// <summary>
    /// Processes the conversation loop until completion or max iterations
    /// </summary>
    private async Task<string> ProcessConversationAsync(ConversationContext context)
    {
        for (var iteration = 0; iteration < ConversationConstants.MaxIterations; iteration++)
        {
            _logger.LogDebug("Azure OpenAI iteration {Iteration}", iteration + 1);

            var response = await context.ChatClient.CompleteChatAsync(context.Messages, context.Options);
            var responseMessage = response.Value.Content.FirstOrDefault();
            var responseText = responseMessage?.Text ?? string.Empty;
            var toolCalls = response.Value.ToolCalls;

            _logger.LogDebug("Received response with {ContentCount} content items, {ToolCallCount} tool calls",
                response.Value.Content.Count, toolCalls?.Count ?? 0);

            if (HasToolCalls(toolCalls))
            {
                await ProcessToolCallsAsync(context, toolCalls!, responseText);
                continue;
            }

            var textResponse = HandleTextResponse(context, responseText);
            if (textResponse != null)
                return textResponse;
        }

        _logger.LogWarning("Maximum iterations reached in Azure OpenAI conversation");
        return ConversationConstants.MaxIterationsReachedMessage;
    }

    /// <summary>
    /// Checks if there are any tool calls in the response
    /// </summary>
    private static bool HasToolCalls(IReadOnlyList<ChatToolCall>? toolCalls) => toolCalls?.Count > 0;

    /// <summary>
    /// Processes tool calls: executes them and handles results
    /// </summary>
    private async Task ProcessToolCallsAsync(
        ConversationContext context,
        IReadOnlyList<ChatToolCall> toolCalls,
        string responseText)
    {
        _logger.LogInformation("Azure OpenAI requested {ToolCallCount} tool calls", toolCalls.Count);

        AddAssistantMessageWithToolCalls(context, toolCalls, responseText);

        var hasAnyFailure = await ExecuteToolCallsAsync(context, toolCalls);
        HandleToolExecutionResults(context, hasAnyFailure);
    }

    /// <summary>
    /// Adds assistant message with tool calls to the conversation
    /// </summary>
    private static void AddAssistantMessageWithToolCalls(
        ConversationContext context,
        IReadOnlyList<ChatToolCall> toolCalls,
        string responseText)
    {
        var assistantMessage = new AssistantChatMessage(responseText);
        foreach (var toolCall in toolCalls.OfType<ChatToolCall>())
            assistantMessage.ToolCalls.Add(toolCall);
        
        context.Messages.Add(assistantMessage);
    }

    /// <summary>
    /// Executes all tool calls and returns whether any failed
    /// </summary>
    private async Task<bool> ExecuteToolCallsAsync(
        ConversationContext context,
        IReadOnlyList<ChatToolCall> toolCalls)
    {
        var hasAnyFailure = false;

        foreach (var functionCall in toolCalls.OfType<ChatToolCall>())
        {
            var failed = await ExecuteSingleToolAsync(context, functionCall);
            if (failed)
                hasAnyFailure = true;
        }

        return hasAnyFailure;
    }

    /// <summary>
    /// Executes a single tool call and adds result to conversation
    /// </summary>
    private async Task<bool> ExecuteSingleToolAsync(
        ConversationContext context,
        ChatToolCall functionCall)
    {
        try
        {
            var arguments = ParseToolArguments(functionCall.FunctionArguments?.ToString());
            var toolResult = await _toolExecutor.ExecuteToolAsync(functionCall.FunctionName, arguments);

            if (_resultAnalyzer.IsSuccess(toolResult))
            {
                AddSuccessfulToolResult(context, functionCall, toolResult);
                return false;
            }

            AddFailedToolResult(context, functionCall, toolResult, arguments);
            return true;
        }
        catch (Exception ex)
        {
            AddToolExecutionError(context, functionCall, ex);
            return true;
        }
    }

    /// <summary>
    /// Adds successful tool result to conversation
    /// </summary>
    private void AddSuccessfulToolResult(
        ConversationContext context,
        ChatToolCall functionCall,
        object toolResult)
    {
        var toolResultJson = JsonSerializer.Serialize(toolResult);
        context.Messages.Add(new ToolChatMessage(functionCall.Id, toolResultJson));
        _logger.LogInformation("Tool {ToolName} executed successfully", functionCall.FunctionName);
    }

    /// <summary>
    /// Adds failed tool result with enhanced error context to conversation
    /// </summary>
    private void AddFailedToolResult(
        ConversationContext context,
        ChatToolCall functionCall,
        object toolResult,
        object? arguments)
    {
        var toolResultJson = JsonSerializer.Serialize(toolResult);
        _logger.LogWarning("Tool {ToolName} failed with result: {Result}", functionCall.FunctionName, toolResultJson);

        var enhancedErrorResult = _errorEnhancer.EnhanceError(toolResult, functionCall.FunctionName, arguments);
        var enhancedResultJson = JsonSerializer.Serialize(enhancedErrorResult);
        context.Messages.Add(new ToolChatMessage(functionCall.Id, enhancedResultJson));
    }

    /// <summary>
    /// Adds tool execution error to conversation
    /// </summary>
    private void AddToolExecutionError(
        ConversationContext context,
        ChatToolCall functionCall,
        Exception ex)
    {
        _logger.LogError(ex, "Error executing tool: {ToolName}", functionCall.FunctionName);

        var errorResult = new
        {
            success = false,
            message = $"Tool execution failed: {ex.Message}",
            errorType = "ToolExecutionError",
            toolName = functionCall.FunctionName,
            suggestion = "Please check the tool parameters and try again with corrected values."
        };

        context.Messages.Add(new ToolChatMessage(functionCall.Id, JsonSerializer.Serialize(errorResult)));
    }

    /// <summary>
    /// Handles tool execution results: updates failure count and tool choice
    /// </summary>
    private void HandleToolExecutionResults(ConversationContext context, bool hasAnyFailure)
    {
        if (hasAnyFailure)
        {
            context.IncrementConsecutiveFailures();
            HandleConsecutiveFailures(context);
        }
        else
        {
            context.ResetConsecutiveFailures();
        }

        UpdateToolChoice(context, hasAnyFailure);
    }

    /// <summary>
    /// Handles consecutive failures by adding system message if threshold reached
    /// </summary>
    private void HandleConsecutiveFailures(ConversationContext context)
    {
        if (!context.HasReachedMaxConsecutiveFailures)
            return;

        context.Messages.Add(new SystemChatMessage(ConversationConstants.MultipleFailuresPrompt));
        context.ResetConsecutiveFailures();
    }

    /// <summary>
    /// Updates tool choice strategy based on execution results
    /// </summary>
    private void UpdateToolChoice(ConversationContext context, bool hasAnyFailure)
    {
        context.Options.ToolChoice = (hasAnyFailure && !context.HasReachedMaxConsecutiveFailures)
            ? ChatToolChoice.CreateRequiredChoice()
            : ChatToolChoice.CreateAutoChoice();
    }

    /// <summary>
    /// Handles text response from AI, checking if it needs tool usage or can be returned
    /// </summary>
    private string? HandleTextResponse(ConversationContext context, string responseText)
    {
        if (string.IsNullOrEmpty(responseText))
        {
            _logger.LogWarning("Received response with no content and no tool calls");
            return ConversationConstants.EmptyResponseMessage;
        }

        if (ShouldForceToolUsageForResponse(context, responseText))
        {
            ForceToolUsageForNextIteration(context, responseText);
            return null; // Continue iteration
        }

        return responseText;
    }

    /// <summary>
    /// Determines if tool usage should be forced based on response content
    /// </summary>
    private bool ShouldForceToolUsageForResponse(ConversationContext context, string responseText)
    {
        var isErrorExplanation = _responseAnalyzer.IsErrorExplanation(responseText);
        
        return (context.ConsecutiveFailures > 0 && isErrorExplanation) ||
               (_toolChoiceStrategy.ShouldForceToolUsage(context.UserMessage) && isErrorExplanation);
    }

    /// <summary>
    /// Forces tool usage for next iteration by adding system message
    /// </summary>
    private void ForceToolUsageForNextIteration(ConversationContext context, string responseText)
    {
        _logger.LogInformation("AI provided explanation instead of action, forcing tool usage");
        
        context.Messages.Add(new AssistantChatMessage(responseText));
        
        var prompt = context.ConsecutiveFailures > 0
            ? ConversationConstants.ErrorExplanationPrompt
            : ConversationConstants.ActionRequiredPrompt;
        
        context.Messages.Add(new SystemChatMessage(prompt));
        context.Options.ToolChoice = ChatToolChoice.CreateRequiredChoice();
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

    /// <summary>
    /// Parses tool arguments from JSON string
    /// </summary>
    private static object? ParseToolArguments(string? rawArgs)
    {
        if (string.IsNullOrWhiteSpace(rawArgs))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(rawArgs);
        }
        catch (JsonException)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(rawArgs);
            }
            catch
            {
                return null;
            }
        }
    }
}
