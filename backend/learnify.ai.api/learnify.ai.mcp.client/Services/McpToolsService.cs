using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Service focused on MCP tools discovery and execution over stdio transport
/// Extracted from LearnifyMcpService to keep responsibilities separated
/// </summary>
public class McpToolsService
{
    private readonly ILogger<McpToolsService> _logger;
    private readonly IConfiguration _configuration;
    private readonly LearnifyMcpService _mcpService; // reuse existing process/stdio plumbing

    public McpToolsService(
        ILogger<McpToolsService> logger,
        IConfiguration configuration,
        LearnifyMcpService mcpService)
    {
        _logger = logger;
        _configuration = configuration;
        _mcpService = mcpService;
    }

    public Task EnsureInitializedAsync() => _mcpService.InitializeAsync();

    /// <summary>
    /// Get tool names list
    /// </summary>
    public Task<List<string>> GetAvailableToolsAsync() => _mcpService.GetAvailableToolsAsync();

    /// <summary>
    /// Get tools with parameter schemas
    /// </summary>
    public Task<Dictionary<string, object>> GetToolsWithParametersAsync() => _mcpService.GetToolsWithParametersAsync();

    /// <summary>
    /// Get a single tool parameter schema
    /// </summary>
    public Task<object?> GetToolParametersAsync(string toolName) => _mcpService.GetToolParametersAsync(toolName);

    /// <summary>
    /// Execute a tool
    /// </summary>
    public Task<object> CallToolAsync(string toolName, object? arguments = null) => _mcpService.CallToolAsync(toolName, arguments);

    /// <summary>
    /// List resources (if supported)
    /// </summary>
    public Task<List<object>> GetAvailableResourcesAsync() => _mcpService.GetAvailableResourcesAsync();
}
