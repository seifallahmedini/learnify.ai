namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Executes tool calls and returns results
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Executes a tool call and returns the result
    /// </summary>
    Task<object> ExecuteToolAsync(string toolName, object? arguments);
}

