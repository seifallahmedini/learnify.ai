using System.Text.Json;
using System.Text.Json.Serialization;
using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Analyzes tool execution results to determine success or failure
/// </summary>
public class ToolResultAnalyzer : IToolResultAnalyzer
{
    public bool IsSuccess(object toolResult)
    {
        try
        {
            var resultJson = JsonSerializer.Serialize(toolResult);
            using var document = JsonDocument.Parse(resultJson);
            var root = document.RootElement;

            // Check for success property
            if (root.TryGetProperty("success", out var successElement))
            {
                return successElement.GetBoolean();
            }

            // If no success property, assume success if no obvious error indicators
            return !root.TryGetProperty("error", out _) && 
                   !root.TryGetProperty("errorType", out _);
        }
        catch
        {
            // If we can't parse, assume success
            return true;
        }
    }
}

