namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Enhances error results with AI-friendly context and suggestions
/// </summary>
public interface IErrorEnhancer
{
    /// <summary>
    /// Enhances an error result with context and retry suggestions
    /// </summary>
    object EnhanceError(object originalResult, string toolName, object? arguments);
}

