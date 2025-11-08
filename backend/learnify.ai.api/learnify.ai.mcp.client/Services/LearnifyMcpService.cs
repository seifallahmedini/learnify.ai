using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Models;
using Learnify.Mcp.Server.Services;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Service for integrating with Learnify.ai MCP Server (in-process tool execution)
/// </summary>
public class LearnifyMcpService : IDisposable
{
    private readonly ILogger<LearnifyMcpService> _logger;
    private readonly IConfiguration _configuration;
    private readonly InProcessMcpToolInvoker _toolInvoker;

    private bool _isInitialized;
    private bool _disposed;

    public LearnifyMcpService(
        ILogger<LearnifyMcpService> logger,
        IConfiguration configuration,
        InProcessMcpToolInvoker toolInvoker)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _toolInvoker = toolInvoker ?? throw new ArgumentNullException(nameof(toolInvoker));
    }

    /// <summary>
    /// Initialize the MCP service (in-process, no external process needed)
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            _logger.LogInformation("MCP Service already initialized");
            return;
        }

        try
        {
            _logger.LogInformation("Initializing in-process MCP Service...");

            // Discover tools (this will cache them for future use)
            var tools = await _toolInvoker.GetAvailableToolsAsync();
            _logger.LogInformation("Discovered {ToolCount} tools", tools.Count);

            _isInitialized = true;
            _logger.LogInformation("In-process MCP Service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize in-process MCP Service");
            throw;
        }
    }

    /// <summary>
    /// Get health status of the service including MCP connection and Azure OpenAI configuration presence
    /// </summary>
    public async Task<ServiceHealthResponse> GetHealthStatusAsync()
    {
        var health = new ServiceHealthResponse
        {
            LastChecked = DateTime.UtcNow
        };

        try
        {
            // MCP connection status (always connected in in-process mode)
            health.McpServer.Connected = _isInitialized;
            health.McpServer.Status = _isInitialized ? "Connected (In-Process)" : "Not Initialized";
            health.McpServer.LastConnected = _isInitialized ? DateTime.UtcNow : null;

            if (_isInitialized)
            {
                try
                {
                    var tools = await GetAvailableToolsAsync();
                    health.AvailableToolsCount = tools.Count;
                    health.AvailableTools = tools;
                }
                catch (Exception ex)
                {
                    health.McpServer.ErrorMessage = ex.Message;
                    health.McpServer.Connected = false;
                    health.McpServer.Status = "Error";
                }
            }

            // Azure OpenAI config presence (no network call here)
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:Key"];
            if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
            {
                health.OpenAI.Connected = true;
                health.OpenAI.Status = "Configured";
                health.OpenAI.LastConnected = DateTime.UtcNow;
            }
            else
            {
                health.OpenAI.Connected = false;
                health.OpenAI.Status = "Not Configured";
                health.OpenAI.ErrorMessage = "Azure OpenAI endpoint or API key not configured";
            }

            health.IsHealthy = health.McpServer.Connected;
            health.Status = health.IsHealthy ? "Healthy" : "Unhealthy";
            if (!health.IsHealthy)
            {
                health.ErrorMessage = health.McpServer.ErrorMessage ?? "MCP server not initialized";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking service health");
            health.IsHealthy = false;
            health.Status = "Error";
            health.ErrorMessage = ex.Message;
        }

        return health;
    }

    /// <summary>
    /// Get the list of available tools from the in-process MCP server
    /// </summary>
    public async Task<List<string>> GetAvailableToolsAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting available tools from in-process MCP server...");
            var toolNames = await _toolInvoker.GetAvailableToolsAsync();

            _logger.LogInformation("Retrieved {ToolCount} tools from in-process MCP server", toolNames.Count);
            LogToolCategories(toolNames);
            return toolNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available tools from in-process MCP server");
            throw new InvalidOperationException("Unable to retrieve tools from in-process MCP server", ex);
        }
    }

    /// <summary>
    /// Get tools including parameter schemas
    /// </summary>
    public async Task<Dictionary<string, object>> GetToolsWithParametersAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting detailed tool information from in-process MCP server...");
            var toolsWithParameters = await _toolInvoker.GetToolsWithParametersAsync();

            _logger.LogInformation("Retrieved {ToolCount} tools with parameter information", toolsWithParameters.Count);
            return toolsWithParameters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tools with parameters from in-process MCP server");
            throw new InvalidOperationException("Unable to retrieve tool parameters from in-process MCP server", ex);
        }
    }

    public async Task<object?> GetToolParametersAsync(string toolName)
    {
        var allTools = await GetToolsWithParametersAsync();
        if (allTools.TryGetValue(toolName, out var toolInfo))
            return toolInfo;
        throw new ArgumentException($"Tool '{toolName}' not found", nameof(toolName));
    }

    public async Task<object> CallToolAsync(string toolName, object? arguments = null)
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Calling in-process tool: {ToolName}", toolName);
            var result = await _toolInvoker.CallToolAsync(toolName, arguments);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call in-process tool: {ToolName}", toolName);
            throw new InvalidOperationException($"Unable to call tool '{toolName}'", ex);
        }
    }

    public async Task<List<object>> GetAvailableResourcesAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Resources not supported in in-process mode");
            return new List<object>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get resources from in-process MCP server");
            return new List<object>();
        }
    }

    #region Private Methods

    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized) await InitializeAsync();
    }

    private void LogToolCategories(List<string> tools)
    {
        var toolsByCategory = GroupToolsByCategory(tools);
        _logger.LogInformation("Available tools by category:");
        foreach (var category in toolsByCategory)
            _logger.LogInformation("  {Category}: {Count} tools", category.Key, category.Value.Count);
    }

    private static Dictionary<string, List<string>> GroupToolsByCategory(List<string> tools)
    {
        return tools.GroupBy(tool =>
        {
            var lowerTool = tool.ToLowerInvariant();
            if (lowerTool.Contains("lesson")) return "Lessons";
            if (lowerTool.Contains("course")) return "Courses";
            if (lowerTool.Contains("category")) return "Categories";
            if (lowerTool.Contains("quiz")) return "Quizzes";
            if (lowerTool.Contains("answer")) return "Answers";
            return "Other";
        }).ToDictionary(g => g.Key, g => g.ToList());
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _isInitialized = false;
            _disposed = true;
        }
    }

    #endregion
}
