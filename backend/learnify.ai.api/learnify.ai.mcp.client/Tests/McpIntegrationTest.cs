using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;

namespace learnify.ai.mcp.client.Tests;

/// <summary>
/// Simple test class to validate MCP integration
/// Run this to test the service without Azure Functions
/// </summary>
public class McpIntegrationTest
{
    public static async Task RunTestAsync(string[] args)
    {
        Console.WriteLine("?? Learnify.ai MCP Client Integration Test");
        Console.WriteLine("==========================================");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Build service provider
        var services = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information))
            .AddSingleton<IConfiguration>(configuration)
            .AddHttpClient()
            .AddScoped<LearnifyMcpService>()
            .AddScoped<McpToolsService>()
            .AddScoped<AiAgentService>()
            .BuildServiceProvider();

        var logger = services.GetRequiredService<ILogger<McpIntegrationTest>>();
        var mcpService = services.GetRequiredService<LearnifyMcpService>();
        var toolsService = services.GetRequiredService<McpToolsService>();
        var aiAgent = services.GetRequiredService<AiAgentService>();

        try
        {
            Console.WriteLine("\n?? Step 1: Initializing MCP Service...");
            await mcpService.InitializeAsync();
            Console.WriteLine("? MCP Service initialized successfully");

            Console.WriteLine("\n?? Step 2: Checking service health...");
            var health = await mcpService.GetHealthStatusAsync();
            Console.WriteLine($"? Health Status: {(health.IsHealthy ? "Healthy" : "Unhealthy")}");
            Console.WriteLine($"   - OpenAI Connected: {health.OpenAI.Connected}");
            Console.WriteLine($"   - MCP Connected: {health.McpServer.Connected}");
            Console.WriteLine($"   - Available Tools: {health.AvailableToolsCount}");

            if (!health.IsHealthy)
            {
                Console.WriteLine($"? Error: {health.ErrorMessage}");
                return;
            }

            Console.WriteLine("\n?? Step 3: Getting available tools...");
            var tools = await toolsService.GetAvailableToolsAsync();
            Console.WriteLine($"? Found {tools.Count} tools:");

            Console.WriteLine("\n?? Step 4: Testing AI request...");
            var testRequest = "What tools are available for course management?";
            var response = await aiAgent.ProcessRequestWithAzureOpenAIAsync(testRequest);
            Console.WriteLine($"? Response received ({response.Length} characters)");
            Console.WriteLine($"   Preview: {response.Substring(0, Math.Min(200, response.Length))}...");

            Console.WriteLine("\n?? All tests completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test failed");
            Console.WriteLine($"? Test failed: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
        }
        finally
        {
            mcpService?.Dispose();
            Console.WriteLine("\n?? Test completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}