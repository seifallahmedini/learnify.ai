using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Models;

namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Service for integrating with Learnify.ai MCP Server (transport + tool management over stdio)
/// </summary>
public class LearnifyMcpService : IDisposable
{
    private readonly ILogger<LearnifyMcpService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private Process? _mcpServerProcess;
    private StreamWriter? _mcpWriter;
    private StreamReader? _mcpReader;
    private bool _isInitialized;
    private bool _disposed;

    private readonly string _mcpServerPath;
    private readonly string _mcpServerExecutable;
    private int _messageId = 1;

    public LearnifyMcpService(
        ILogger<LearnifyMcpService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _mcpServerPath = _configuration["McpServer:Path"] ?? string.Empty;
        if (string.IsNullOrEmpty(_mcpServerPath))
        {
            _mcpServerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "learnify.ai.mcp.server");
        }
        if (!Path.IsPathRooted(_mcpServerPath))
        {
            _mcpServerPath = Path.GetFullPath(_mcpServerPath);
        }

        _mcpServerExecutable = _configuration["McpServer:Executable"] ?? "dotnet";
        _logger.LogInformation("MCP Server configured at: {Path}", _mcpServerPath);
    }

    /// <summary>
    /// Initialize the MCP client connection to the server
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
            _logger.LogInformation("Initializing MCP Service...");
            await StartMcpServerProcessAsync();
            await InitializeMcpConnectionAsync();
            _isInitialized = true;
            _logger.LogInformation("MCP Service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP Service");
            await CleanupAsync();
            throw;
        }
    }

    /// <summary>
    /// Get health status of the service including MCP connection and Azure OpenAI configuration presence
    /// </summary>
    public async Task<ServiceHealthResponse> GetHealthStatusAsync()
    {
        // Ensure we don't trigger initialization here; just report current state + config
        var health = new ServiceHealthResponse
        {
            LastChecked = DateTime.UtcNow
        };

        try
        {
            // MCP connection status
            health.McpServer.Connected = _isInitialized && _mcpServerProcess is { HasExited: false } && _mcpWriter != null && _mcpReader != null;
            health.McpServer.Status = health.McpServer.Connected ? "Connected" : "Disconnected";
            health.McpServer.LastConnected = health.McpServer.Connected ? DateTime.UtcNow : null;

            if (health.McpServer.Connected)
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
                health.ErrorMessage = health.McpServer.ErrorMessage ?? "MCP server connection failed";
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
    /// Get the list of available tools from the MCP server
    /// </summary>
    public async Task<List<string>> GetAvailableToolsAsync()
    {
        await EnsureInitializedAsync();

        try
        {
            _logger.LogInformation("Requesting available tools from MCP server...");
            if (_mcpWriter == null || _mcpReader == null)
                throw new InvalidOperationException("MCP connection is not established");

            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/list",
                @params = new { }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();
            _logger.LogDebug("Sent tools/list request: {Request}", requestJson);

            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("No response received from MCP server");

            _logger.LogDebug("Received response: {Response}", responseJson);

            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            var toolNames = new List<string>();
            if (root.TryGetProperty("result", out var result) && result.TryGetProperty("tools", out var tools))
            {
                foreach (var tool in tools.EnumerateArray())
                {
                    if (tool.TryGetProperty("name", out var nameProperty))
                    {
                        var name = nameProperty.GetString();
                        if (!string.IsNullOrEmpty(name))
                            toolNames.Add(name);
                    }
                }
            }

            _logger.LogInformation("Retrieved {ToolCount} tools from MCP server", toolNames.Count);
            LogToolCategories(toolNames);
            return toolNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available tools from MCP server");
            throw new InvalidOperationException("Unable to retrieve tools from MCP server", ex);
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
            _logger.LogInformation("Requesting detailed tool information from MCP server...");
            if (_mcpWriter == null || _mcpReader == null)
                throw new InvalidOperationException("MCP connection is not established");

            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/list",
                @params = new { }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();
            _logger.LogDebug("Sent tools/list request: {Request}", requestJson);

            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("No response received from MCP server");

            _logger.LogDebug("Received response: {Response}", responseJson);

            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            var toolsWithParameters = new Dictionary<string, object>();
            if (root.TryGetProperty("result", out var result) && result.TryGetProperty("tools", out var tools))
            {
                foreach (var tool in tools.EnumerateArray())
                {
                    var toolInfo = new Dictionary<string, object>();
                    if (tool.TryGetProperty("name", out var nameProperty))
                    {
                        var name = nameProperty.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (tool.TryGetProperty("description", out var descProperty))
                                toolInfo["description"] = descProperty.GetString() ?? string.Empty;
                            if (tool.TryGetProperty("inputSchema", out var schemaProperty))
                                toolInfo["inputSchema"] = JsonSerializer.Deserialize<object>(schemaProperty.GetRawText())!;
                            toolsWithParameters[name] = toolInfo;
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {ToolCount} tools with parameter information", toolsWithParameters.Count);
            return toolsWithParameters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tools with parameters from MCP server");
            throw new InvalidOperationException("Unable to retrieve tool parameters from MCP server", ex);
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
            _logger.LogInformation("Calling tool: {ToolName} with arguments: {Arguments}", toolName, JsonSerializer.Serialize(arguments ?? new object()));
            if (_mcpWriter == null || _mcpReader == null)
                throw new InvalidOperationException("MCP connection is not established");

            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "tools/call",
                @params = new { name = toolName, arguments = arguments ?? new object() }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();
            _logger.LogDebug("Sent tools/call request: {Request}", requestJson);

            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("No response received from MCP server");

            _logger.LogDebug("Received tool call response: {Response}", responseJson);

            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server returned error: {errorMessage}");
            }

            if (root.TryGetProperty("result", out var result))
                return JsonSerializer.Deserialize<object>(result.GetRawText()) ?? new object();

            throw new InvalidOperationException("Invalid response format from MCP server");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call tool: {ToolName}", toolName);
            throw new InvalidOperationException($"Unable to call tool '{toolName}'", ex);
        }
    }

    public async Task<List<object>> GetAvailableResourcesAsync()
    {
        await EnsureInitializedAsync();
        try
        {
            _logger.LogInformation("Requesting available resources from MCP server...");
            if (_mcpWriter == null || _mcpReader == null)
                throw new InvalidOperationException("MCP connection is not established");

            var request = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "resources/list",
                @params = new { }
            };

            var requestJson = JsonSerializer.Serialize(request);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();
            _logger.LogDebug("Sent resources/list request: {Request}", requestJson);

            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("No response received from MCP server");

            _logger.LogDebug("Received resources response: {Response}", responseJson);

            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                _logger.LogWarning("MCP server returned error for resources/list: {Error}", errorMessage);
                return new List<object>();
            }

            var resources = new List<object>();
            if (root.TryGetProperty("result", out var result) && result.TryGetProperty("resources", out var resourcesArray))
            {
                foreach (var resource in resourcesArray.EnumerateArray())
                {
                    var resourceObj = JsonSerializer.Deserialize<object>(resource.GetRawText());
                    if (resourceObj != null)
                        resources.Add(resourceObj);
                }
            }

            _logger.LogInformation("Retrieved {ResourceCount} resources from MCP server", resources.Count);
            return resources;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get resources from MCP server (might not be supported)");
            return new List<object>();
        }
    }

    #region Private Methods

    private async Task StartMcpServerProcessAsync()
    {
        try
        {
            _logger.LogInformation("Starting MCP server process at: {Path}", _mcpServerPath);
            if (!Directory.Exists(_mcpServerPath))
                throw new DirectoryNotFoundException($"MCP server directory not found: {_mcpServerPath}");

            var projectFile = Path.Combine(_mcpServerPath, "learnify.ai.mcp.server.csproj");
            if (!File.Exists(projectFile))
                throw new FileNotFoundException($"MCP server project file not found: {projectFile}");

            var startInfo = new ProcessStartInfo
            {
                FileName = _mcpServerExecutable,
                Arguments = "run --configuration Release --no-build --verbosity quiet --nologo",
                WorkingDirectory = _mcpServerPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _mcpServerProcess = new Process { StartInfo = startInfo };
            _mcpServerProcess.EnableRaisingEvents = true;
            _mcpServerProcess.Exited += (sender, e) =>
            {
                _logger.LogWarning("MCP server process exited unexpectedly with code: {ExitCode}", _mcpServerProcess?.ExitCode);
            };
            _mcpServerProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) _logger.LogDebug("MCP Server: {Message}", e.Data);
            };

            if (!_mcpServerProcess.Start())
                throw new InvalidOperationException("Failed to start MCP server process");

            _mcpServerProcess.BeginErrorReadLine();

            _mcpWriter = new StreamWriter(_mcpServerProcess.StandardInput.BaseStream, System.Text.Encoding.UTF8)
            { AutoFlush = true };
            _mcpReader = new StreamReader(_mcpServerProcess.StandardOutput.BaseStream, System.Text.Encoding.UTF8);

            await Task.Delay(3000);
            if (_mcpServerProcess.HasExited)
            {
                var exitCode = _mcpServerProcess.ExitCode;
                throw new InvalidOperationException($"MCP server process exited immediately with code: {exitCode}.");
            }

            _logger.LogInformation("MCP server process started successfully (PID: {ProcessId})", _mcpServerProcess.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MCP server process. Working directory: {WorkingDirectory}, Executable: {Executable}", _mcpServerPath, _mcpServerExecutable);
            throw;
        }
    }

    private async Task InitializeMcpConnectionAsync()
    {
        try
        {
            if (_mcpWriter == null || _mcpReader == null)
                throw new InvalidOperationException("MCP streams are not available");

            _logger.LogInformation("Initializing MCP connection...");

            var initRequest = new
            {
                jsonrpc = "2.0",
                id = _messageId++,
                method = "initialize",
                @params = new
                {
                    clientInfo = new { name = "learnify-ai-client", version = "1.0.0" },
                    protocolVersion = "2024-11-05"
                }
            };

            var requestJson = JsonSerializer.Serialize(initRequest);
            await _mcpWriter.WriteLineAsync(requestJson);
            await _mcpWriter.FlushAsync();
            _logger.LogDebug("Sent initialize request: {Request}", requestJson);

            var responseJson = await _mcpReader.ReadLineAsync();
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("No initialization response received from MCP server");

            _logger.LogDebug("Received initialize response: {Response}", responseJson);

            using var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;
            if (root.TryGetProperty("error", out var errorElement))
            {
                var errorMessage = errorElement.GetProperty("message").GetString();
                throw new InvalidOperationException($"MCP server initialization failed: {errorMessage}");
            }

            var initializedNotification = new
            {
                jsonrpc = "2.0",
                method = "notifications/initialized",
                @params = new { }
            };

            var notificationJson = JsonSerializer.Serialize(initializedNotification);
            await _mcpWriter.WriteLineAsync(notificationJson);
            await _mcpWriter.FlushAsync();
            _logger.LogInformation("MCP connection initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP connection");
            throw;
        }
    }

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

    private async Task CleanupAsync()
    {
        try
        {
            _mcpWriter?.Dispose();
            _mcpWriter = null;
            _mcpReader?.Dispose();
            _mcpReader = null;

            if (_mcpServerProcess != null && !_mcpServerProcess.HasExited)
            {
                _mcpServerProcess.Kill();
                await _mcpServerProcess.WaitForExitAsync();
            }

            _mcpServerProcess?.Dispose();
            _mcpServerProcess = null;
            _isInitialized = false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during cleanup");
        }
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
            CleanupAsync().Wait();
            _disposed = true;
        }
    }

    #endregion
}
