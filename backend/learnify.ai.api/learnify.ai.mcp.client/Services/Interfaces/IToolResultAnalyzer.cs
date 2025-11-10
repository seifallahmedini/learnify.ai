namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Analyzes tool execution results to determine success or failure
/// </summary>
public interface IToolResultAnalyzer
{
    /// <summary>
    /// Determines if a tool result indicates success
    /// </summary>
    bool IsSuccess(object toolResult);
}

