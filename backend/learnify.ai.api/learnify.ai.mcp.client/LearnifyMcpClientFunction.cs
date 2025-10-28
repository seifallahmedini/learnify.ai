using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;
using learnify.ai.mcp.client.Models;
using System.Text.Json;
using System.Diagnostics;

namespace learnify.ai.mcp.client;

public class LearnifyMcpClientFunction
{
    private readonly ILogger<LearnifyMcpClientFunction> _logger;
    private readonly McpToolsService _toolsService;
    private readonly AiAgentService _aiAgent;

    public LearnifyMcpClientFunction(
        ILogger<LearnifyMcpClientFunction> logger,
        McpToolsService toolsService,
        AiAgentService aiAgent)
    {
        _logger = logger;
        _toolsService = toolsService;
        _aiAgent = aiAgent;
    }

    [Function("Health")]
    public async Task<IActionResult> HealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Health check requested");
            
            // Reuse original health via base LearnifyMcpService if needed, otherwise basic ping
            await _toolsService.EnsureInitializedAsync();
            return new OkObjectResult(new { ok = true, message = "MCP initialized" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    [Function("GetAvailableTools")]
    public async Task<IActionResult> GetAvailableTools([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Available tools request received");

            await _toolsService.EnsureInitializedAsync();
            var tools = await _toolsService.GetAvailableToolsAsync();

            return new OkObjectResult(new
            {
                success = true,
                toolsCount = tools.Count,
                tools
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available tools");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    [Function("GetToolsWithParameters")]
    public async Task<IActionResult> GetToolsWithParameters([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            await _toolsService.EnsureInitializedAsync();
            var toolsWithParams = await _toolsService.GetToolsWithParametersAsync();
            return new OkObjectResult(new { success = true, toolsCount = toolsWithParams.Count, tools = toolsWithParams });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tools with parameters");
            return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        }
    }

    [Function("GetToolParameters")]
    public async Task<IActionResult> GetToolParameters([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var toolName = req.Query["toolName"].ToString();
            if (string.IsNullOrEmpty(toolName))
            {
                return new BadRequestObjectResult(new { error = "toolName parameter is required" });
            }

            await _toolsService.EnsureInitializedAsync();
            var toolParams = await _toolsService.GetToolParametersAsync(toolName);
            return new OkObjectResult(new { success = true, toolName, parameters = toolParams });
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

    [Function("CallTool")]
    public async Task<IActionResult> CallTool([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            using var document = JsonDocument.Parse(requestBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("toolName", out var toolNameElement))
                return new BadRequestObjectResult(new { error = "toolName is required" });

            var toolName = toolNameElement.GetString();
            if (string.IsNullOrEmpty(toolName))
                return new BadRequestObjectResult(new { error = "toolName cannot be empty" });

            object? arguments = null;
            if (root.TryGetProperty("arguments", out var argumentsElement))
                arguments = JsonSerializer.Deserialize<object>(argumentsElement.GetRawText());

            await _toolsService.EnsureInitializedAsync();
            var result = await _toolsService.CallToolAsync(toolName, arguments);
            
            sw.Stop();
            return new OkObjectResult(new
            {
                success = true,
                response = JsonSerializer.Serialize(result),
                processingTime = sw.Elapsed,
                tool = toolName
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error executing tool");
            return new ObjectResult(new { error = ex.Message, processingTime = sw.Elapsed }) { StatusCode = 500 };
        }
    }

    [Function("AzureOpenAIEducationalAssistant")]
    public async Task<IActionResult> AzureOpenAIEducationalAssistant([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<EducationalRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request == null || string.IsNullOrEmpty(request.Message))
                return new BadRequestObjectResult(new { error = "Message is required" });

            await _toolsService.EnsureInitializedAsync();
            var result = await _aiAgent.ProcessRequestWithAzureOpenAIAsync(request.Message, request.SystemPrompt);
            
            sw.Stop();
            return new OkObjectResult(new { success = true, response = result, processingTime = sw.Elapsed });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error processing Azure OpenAI educational request");
            return new ObjectResult(new { error = ex.Message, processingTime = sw.Elapsed }) { StatusCode = 500 };
        }
    }

    [Function("IntelligentEducationalAssistant")]
    public async Task<IActionResult> IntelligentEducationalAssistant([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<EducationalRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request == null || string.IsNullOrEmpty(request.Message))
                return new BadRequestObjectResult(new { error = "Message is required" });

            await _toolsService.EnsureInitializedAsync();
            var result = await _aiAgent.ProcessIntelligentEducationalRequestAsync(request.Message);
            
            sw.Stop();
            return new OkObjectResult(new { success = true, response = result, processingTime = sw.Elapsed });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error processing intelligent educational request");
            return new ObjectResult(new { error = ex.Message, processingTime = sw.Elapsed }) { StatusCode = 500 };
        }
    }
}
