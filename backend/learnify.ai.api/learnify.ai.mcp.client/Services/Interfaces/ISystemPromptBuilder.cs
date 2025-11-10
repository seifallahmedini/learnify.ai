namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Builds system prompts for AI conversations
/// </summary>
public interface ISystemPromptBuilder
{
    /// <summary>
    /// Builds an enhanced system prompt with tool usage guidelines
    /// </summary>
    string BuildSystemPrompt(string? basePrompt = null);
}

