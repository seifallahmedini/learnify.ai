using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Factory for creating Azure OpenAI chat clients
/// </summary>
public interface IAzureOpenAIClientFactory
{
    /// <summary>
    /// Creates a chat client for Azure OpenAI
    /// </summary>
    ChatClient CreateChatClient();
}

