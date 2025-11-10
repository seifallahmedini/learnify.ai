namespace learnify.ai.mcp.client.Services.Interfaces;

/// <summary>
/// Converts MCP tools to OpenAI format
/// </summary>
public interface IToolConverter
{
    /// <summary>
    /// Converts MCP tools dictionary to OpenAI ChatTool format
    /// </summary>
    List<OpenAI.Chat.ChatTool> ConvertToOpenAIFormat(Dictionary<string, object> mcpTools);
}

