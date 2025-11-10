using OpenAI.Chat;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Context object to hold conversation state during processing
/// </summary>
internal class ConversationContext
{
    public List<ChatMessage> Messages { get; set; } = new();
    public ChatCompletionOptions Options { get; set; } = new();
    public int ConsecutiveFailures { get; set; }
    public string UserMessage { get; set; } = string.Empty;
    public ChatClient ChatClient { get; set; } = null!;

    private const int MaxConsecutiveFailures = 3;

    public bool HasReachedMaxConsecutiveFailures => ConsecutiveFailures >= MaxConsecutiveFailures;

    public void ResetConsecutiveFailures() => ConsecutiveFailures = 0;

    public void IncrementConsecutiveFailures() => ConsecutiveFailures++;
}

