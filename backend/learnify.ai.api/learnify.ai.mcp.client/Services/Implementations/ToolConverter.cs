using System.Text.Json;
using Azure;
using learnify.ai.mcp.client.Services.Interfaces;
using OpenAI.Chat;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Converts MCP tools to OpenAI format
/// </summary>
public class ToolConverter : IToolConverter
{
    public List<ChatTool> ConvertToOpenAIFormat(Dictionary<string, object> mcpTools)
    {
        var openAITools = new List<ChatTool>();

        foreach (var (toolName, toolInfo) in mcpTools)
        {
            try
            {
                if (toolInfo is Dictionary<string, object> info)
                {
                    var description = info.GetValueOrDefault("description", $"Educational management tool: {toolName}")?.ToString() 
                        ?? $"Tool: {toolName}";

                    object? parametersObj = info.GetValueOrDefault("inputSchema", null);
                    parametersObj ??= new { type = "object", properties = new { }, additionalProperties = true };

                    var parametersJson = JsonSerializer.Serialize(parametersObj);
                    var parametersBinary = BinaryData.FromString(parametersJson);

                    openAITools.Add(ChatTool.CreateFunctionTool(toolName, description, parametersBinary));
                }
            }
            catch
            {
                var basicSchema = new { type = "object", properties = new { }, additionalProperties = true };
                openAITools.Add(ChatTool.CreateFunctionTool(toolName, $"Educational management tool: {toolName}", 
                    BinaryData.FromObjectAsJson(basicSchema)));
            }
        }

        return openAITools;
    }
}

