using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;
using learnify.ai.mcp.client.Models;
using System.Text.Json;
using System.Diagnostics;

namespace learnify.ai.mcp.client;

/// <summary>
/// Azure Functions for Learnify.ai MCP Client
/// Provides AI-powered educational management through conversational interface
/// </summary>
public class LearnifyMcpClientFunction
{
    private readonly ILogger<LearnifyMcpClientFunction> _logger;
    private readonly LearnifyMcpService _mcpService;

    public LearnifyMcpClientFunction(ILogger<LearnifyMcpClientFunction> logger, LearnifyMcpService mcpService)
    {
        _logger = logger;
        _mcpService = mcpService;
    }

    /// <summary>
    /// Health check endpoint for service monitoring
    /// </summary>
    [Function("Health")]
    public async Task<IActionResult> HealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Health check requested");
            
            var healthStatus = await _mcpService.GetHealthStatusAsync();
            
            return new OkObjectResult(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Get available tools endpoint
    /// Returns list of available MCP tools
    /// </summary>
    [Function("GetAvailableTools")]
    public async Task<IActionResult> GetAvailableTools(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Available tools request received");

            await _mcpService.InitializeAsync();
            var tools = await _mcpService.GetAvailableToolsAsync();

            return new OkObjectResult(new
            {
                success = true,
                toolsCount = tools.Count,
                tools = tools,
                categories = new
                {
                    lessons = tools.Count(t => t.ToLowerInvariant().Contains("lesson")),
                    courses = tools.Count(t => t.ToLowerInvariant().Contains("course")),
                    categories = tools.Count(t => t.ToLowerInvariant().Contains("category")),
                    quizzes = tools.Count(t => t.ToLowerInvariant().Contains("quiz")),
                    answers = tools.Count(t => t.ToLowerInvariant().Contains("answer"))
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available tools");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Get detailed tool information including parameters
    /// </summary>
    [Function("GetToolsWithParameters")]
    public async Task<IActionResult> GetToolsWithParameters(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Tool parameters request received");

            await _mcpService.InitializeAsync();
            var toolsWithParams = await _mcpService.GetToolsWithParametersAsync();

            return new OkObjectResult(new
            {
                success = true,
                toolsCount = toolsWithParams.Count,
                tools = toolsWithParams
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tools with parameters");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Get parameters for a specific tool
    /// </summary>
    [Function("GetToolParameters")]
    public async Task<IActionResult> GetToolParameters(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var toolName = req.Query["toolName"].ToString();
            if (string.IsNullOrEmpty(toolName))
            {
                return new BadRequestObjectResult(new { error = "toolName parameter is required" });
            }

            _logger.LogInformation("Tool parameters request for: {ToolName}", toolName);

            await _mcpService.InitializeAsync();
            var toolParams = await _mcpService.GetToolParametersAsync(toolName);

            return new OkObjectResult(new
            {
                success = true,
                toolName = toolName,
                parameters = toolParams
            });
        }
        catch (ArgumentException ex)
        {
            return new NotFoundObjectResult(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tool parameters");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Execute a specific tool with parameters
    /// </summary>
    [Function("CallTool")]
    public async Task<IActionResult> CallTool(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Tool execution request received");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            using var document = JsonDocument.Parse(requestBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("toolName", out var toolNameElement))
            {
                return new BadRequestObjectResult(new { error = "toolName is required" });
            }

            var toolName = toolNameElement.GetString();
            if (string.IsNullOrEmpty(toolName))
            {
                return new BadRequestObjectResult(new { error = "toolName cannot be empty" });
            }

            object? arguments = null;
            if (root.TryGetProperty("arguments", out var argumentsElement))
            {
                arguments = JsonSerializer.Deserialize<object>(argumentsElement.GetRawText());
            }

            await _mcpService.InitializeAsync();
            var result = await _mcpService.CallToolAsync(toolName, arguments);
            
            stopwatch.Stop();

            var response = new EducationalResponse
            {
                Success = true,
                Response = JsonSerializer.Serialize(result),
                ProcessingTime = stopwatch.Elapsed,
                ToolsUsed = new List<string> { toolName },
                Metadata = new Dictionary<string, object>
                {
                    { "toolName", toolName },
                    { "argumentsProvided", arguments != null }
                }
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error executing tool");
            
            var response = new EducationalResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProcessingTime = stopwatch.Elapsed
            };

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Get available resources from MCP server
    /// </summary>
    [Function("GetAvailableResources")]
    public async Task<IActionResult> GetAvailableResources(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Available resources request received");

            await _mcpService.InitializeAsync();
            var resources = await _mcpService.GetAvailableResourcesAsync();

            return new OkObjectResult(new
            {
                success = true,
                resourcesCount = resources.Count,
                resources = resources
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available resources");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Intelligent educational assistant endpoint using Azure OpenAI with MCP tools integration and Microsoft Learn knowledge
    /// </summary>
    [Function("IntelligentEducationalAssistant")]
    public async Task<IActionResult> IntelligentEducationalAssistant(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Intelligent educational assistant request received");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<EducationalRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return new BadRequestObjectResult(new { error = "Message is required" });
            }

            _logger.LogInformation("Processing intelligent educational request: {Message}", request.Message);

            await _mcpService.InitializeAsync();

            // Use the intelligent educational processing method with Azure OpenAI and Microsoft Learn integration
            var result = await _mcpService.ProcessIntelligentEducationalRequestAsync(request.Message);
            
            stopwatch.Stop();

            var response = new EducationalResponse
            {
                Success = true,
                Response = result,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "requestType", request.RequestType.ToString() },
                    { "intelligentProcessing", true },
                    { "azureOpenAIEnabled", true },
                    { "microsoftLearnIntegration", true },
                    { "mcpToolsAvailable", true }
                }
            };

            _logger.LogInformation("Intelligent educational request processed successfully in {ProcessingTime}ms", 
                stopwatch.ElapsedMilliseconds);

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing intelligent educational request");
            
            var response = new EducationalResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "intelligentProcessing", false },
                    { "fallbackUsed", true },
                    { "errorOccurred", true }
                }
            };

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Azure OpenAI educational assistant endpoint with MCP tools integration
    /// </summary>
    [Function("AzureOpenAIEducationalAssistant")]
    public async Task<IActionResult> AzureOpenAIEducationalAssistant(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Azure OpenAI educational assistant request received");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<EducationalRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return new BadRequestObjectResult(new { error = "Message is required" });
            }

            _logger.LogInformation("Processing Azure OpenAI educational request: {Message}", request.Message);

            await _mcpService.InitializeAsync();

            // Use Azure OpenAI with MCP tools integration
            var result = await _mcpService.ProcessRequestWithAzureOpenAIAsync(request.Message, request.SystemPrompt);
            
            stopwatch.Stop();

            var response = new EducationalResponse
            {
                Success = true,
                Response = result,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "requestType", request.RequestType.ToString() },
                    { "azureOpenAIProcessing", true },
                    { "mcpToolsIntegration", true },
                    { "systemPromptUsed", !string.IsNullOrEmpty(request.SystemPrompt) }
                }
            };

            _logger.LogInformation("Azure OpenAI educational request processed successfully in {ProcessingTime}ms", 
                stopwatch.ElapsedMilliseconds);

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing Azure OpenAI educational request");
            
            var response = new EducationalResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "azureOpenAIProcessing", false },
                    { "errorOccurred", true }
                }
            };

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Educational analytics endpoint
    /// Specialized for analytics queries and reporting
    /// </summary>
    [Function("EducationalAnalytics")]
    public async Task<IActionResult> EducationalAnalytics(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Educational analytics request received");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<AnalyticsRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null || string.IsNullOrEmpty(request.Query))
            {
                return new BadRequestObjectResult(new { error = "Query is required" });
            }

            await _mcpService.InitializeAsync();

            // Build context-aware analytics query
            var contextualQuery = BuildAnalyticsQuery(request);
            var result = await _mcpService.GetEducationalAnalyticsAsync(contextualQuery);
            
            stopwatch.Stop();

            var response = new EducationalResponse
            {
                Success = true,
                Response = result,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "analyticsType", request.Type.ToString() },
                    { "courseId", request.CourseId ?? 0 },
                    { "categoryId", request.CategoryId ?? 0 },
                    { "quizId", request.QuizId ?? 0 }
                }
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing analytics request");
            
            var response = new EducationalResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProcessingTime = stopwatch.Elapsed
            };

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// General educational assistant endpoint
    /// Handles any educational request using basic MCP tools processing
    /// </summary>
    [Function("EducationalAssistant")]
    public async Task<IActionResult> EducationalAssistant(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Educational assistant request received");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<EducationalRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return new BadRequestObjectResult(new { error = "Message is required" });
            }

            await _mcpService.InitializeAsync();

            var result = await _mcpService.ProcessEducationalRequestAsync(request.Message);
            
            stopwatch.Stop();

            var response = new EducationalResponse
            {
                Success = true,
                Response = result,
                ProcessingTime = stopwatch.Elapsed,
                Metadata = new Dictionary<string, object>
                {
                    { "requestType", request.RequestType.ToString() },
                    { "basicProcessing", true }
                }
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing educational request");
            
            var response = new EducationalResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ProcessingTime = stopwatch.Elapsed
            };

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }

    #region Private Helper Methods

    private static string BuildAnalyticsQuery(AnalyticsRequest request)
    {
        var query = request.Query;
        
        if (request.CourseId.HasValue)
            query += $" (Focus on course ID: {request.CourseId})";
        
        if (request.CategoryId.HasValue)
            query += $" (Focus on category ID: {request.CategoryId})";
        
        if (request.QuizId.HasValue)
            query += $" (Focus on quiz ID: {request.QuizId})";
        
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            query += $" (Date range: {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd})";
        }

        return query;
    }

    #endregion
}
