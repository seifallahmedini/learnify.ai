namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Analyzes AI responses to determine if they are error explanations
/// </summary>
public interface IResponseAnalyzer
{
    /// <summary>
    /// Determines if a response is just explaining an error instead of taking action
    /// </summary>
    bool IsErrorExplanation(string response);
}

