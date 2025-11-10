namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Determines when tool usage should be forced based on user messages
/// </summary>
public interface IToolChoiceStrategy
{
    /// <summary>
    /// Determines if tool usage should be forced for a given user message
    /// </summary>
    bool ShouldForceToolUsage(string userMessage);
}

