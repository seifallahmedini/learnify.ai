using System.Text.Json;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Executes tool calls via MCP tools service
/// </summary>
public class ToolExecutor : IToolExecutor
{
    private readonly McpToolsService _toolsService;
    private readonly ILogger<ToolExecutor> _logger;

    public ToolExecutor(
        McpToolsService toolsService,
        ILogger<ToolExecutor> logger)
    {
        _toolsService = toolsService;
        _logger = logger;
    }

    public async Task<object> ExecuteToolAsync(string toolName, object? arguments)
    {
        _logger.LogInformation("Executing tool: {ToolName} with arguments: {Arguments}",
            toolName, arguments != null ? JsonSerializer.Serialize(arguments) : "null");

        return await _toolsService.CallToolAsync(toolName, arguments);
    }
}

