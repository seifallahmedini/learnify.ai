using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;
using System.Text.Json;

namespace learnify.ai.mcp.client;

/// <summary>
/// Console application to test MCP connection
/// </summary>
public class TestConsole
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("?? Learnify.ai MCP Client Test Console");
        Console.WriteLine("=====================================");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Validate MCP server path before starting
        var mcpServerPath = configuration["McpServer:Path"];
        if (string.IsNullOrEmpty(mcpServerPath))
        {
            Console.WriteLine("? Error: McpServer:Path not configured in local.settings.json");
            Console.WriteLine("Please set the correct path to the MCP server directory.");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        if (!Directory.Exists(mcpServerPath))
        {
            Console.WriteLine($"? Error: MCP server directory not found: {mcpServerPath}");
            Console.WriteLine("Please check the path in local.settings.json");
            Console.WriteLine($"Current working directory: {Directory.GetCurrentDirectory()}");
            
            // Try to find the correct path
            var currentDir = Directory.GetCurrentDirectory();
            var possiblePaths = new[]
            {
                Path.Combine(currentDir, "..", "learnify.ai.mcp.server"),
                Path.Combine(currentDir, "learnify.ai.mcp.server"),
                Path.GetFullPath(Path.Combine(currentDir, "..", "learnify.ai.mcp.server"))
            };
            
            Console.WriteLine("\nLooking for MCP server in these locations:");
            foreach (var path in possiblePaths)
            {
                var exists = Directory.Exists(path);
                Console.WriteLine($"  {(exists ? "?" : "?")} {path}");
                if (exists)
                {
                    Console.WriteLine($"\nSuggestion: Update McpServer:Path to: {path}");
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        var projectFile = Path.Combine(mcpServerPath, "learnify.ai.mcp.server.csproj");
        if (!File.Exists(projectFile))
        {
            Console.WriteLine($"? Error: MCP server project file not found: {projectFile}");
            Console.WriteLine("Please ensure the MCP server project is built and available.");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"? MCP server path validated: {mcpServerPath}");

        // Build service provider
        var services = new ServiceCollection()
            .AddLogging(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<IConfiguration>(configuration)
            .AddHttpClient()
            .AddScoped<LearnifyMcpService>()
            .BuildServiceProvider();

        var logger = services.GetRequiredService<ILogger<TestConsole>>();
        var mcpService = services.GetRequiredService<LearnifyMcpService>();

        try
        {
            Console.WriteLine("\n?? Step 1: Initializing MCP Service...");
            await mcpService.InitializeAsync();
            Console.WriteLine("? MCP Service initialized successfully");

            Console.WriteLine("\n?? Step 2: Checking service health...");
            var health = await mcpService.GetHealthStatusAsync();
            Console.WriteLine($"? Health Status: {health.Status}");
            Console.WriteLine($"   - MCP Connected: {health.McpServer.Connected}");
            Console.WriteLine($"   - Available Tools: {health.AvailableToolsCount}");

            if (!health.IsHealthy)
            {
                Console.WriteLine($"? Error: {health.ErrorMessage}");
                return;
            }

            Console.WriteLine("\n?? Step 3: Getting available tools...");
            var tools = await mcpService.GetAvailableToolsAsync();
            Console.WriteLine($"? Found {tools.Count} tools:");
            
            // Group tools by category for better display
            var toolsByCategory = tools.GroupBy(tool =>
            {
                var lowerTool = tool.ToLowerInvariant();
                if (lowerTool.Contains("lesson")) return "?? Lessons";
                if (lowerTool.Contains("course")) return "?? Courses";
                if (lowerTool.Contains("category")) return "?? Categories";
                if (lowerTool.Contains("quiz")) return "? Quizzes";
                if (lowerTool.Contains("answer")) return "? Answers";
                return "?? Other";
            }).OrderBy(g => g.Key);

            foreach (var category in toolsByCategory)
            {
                Console.WriteLine($"\n   {category.Key} ({category.Count()} tools):");
                foreach (var tool in category.Take(5)) // Show first 5 tools per category
                {
                    Console.WriteLine($"     - {tool}");
                }
                if (category.Count() > 5)
                {
                    Console.WriteLine($"     ... and {category.Count() - 5} more");
                }
            }

            Console.WriteLine($"\n?? Total: {tools.Count} tools available across {toolsByCategory.Count()} categories");

            Console.WriteLine("\n?? Step 4: Getting tool parameters...");
            var toolsWithParams = await mcpService.GetToolsWithParametersAsync();
            Console.WriteLine($"? Found {toolsWithParams.Count} tools with parameters:");
            
            // Show detailed information for first few tools
            var toolsToShow = toolsWithParams.Take(3);
            foreach (var tool in toolsToShow)
            {
                Console.WriteLine($"\n   ?? Tool: {tool.Key}");
                if (tool.Value is Dictionary<string, object> toolInfo)
                {
                    if (toolInfo.TryGetValue("description", out var desc))
                    {
                        Console.WriteLine($"      Description: {desc}");
                    }
                    if (toolInfo.TryGetValue("inputSchema", out var schema))
                    {
                        Console.WriteLine($"      Parameters: {JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true })}");
                    }
                }
            }

            if (toolsWithParams.Count > 3)
            {
                Console.WriteLine($"\n   ... and {toolsWithParams.Count - 3} more tools with parameters");
            }

            Console.WriteLine("\n?? Step 5: Testing tool execution...");
            
            // Try to execute a simple tool (like get_lessons or get_courses)
            var availableTools = await mcpService.GetAvailableToolsAsync();
            var testTool = availableTools.FirstOrDefault(t => 
                t.ToLowerInvariant().Contains("get") && 
                (t.ToLowerInvariant().Contains("lesson") || t.ToLowerInvariant().Contains("course")));

            if (!string.IsNullOrEmpty(testTool))
            {
                Console.WriteLine($"   Testing tool: {testTool}");
                try
                {
                    var toolResult = await mcpService.CallToolAsync(testTool, new { });
                    Console.WriteLine($"   ? Tool executed successfully");
                    Console.WriteLine($"   Result: {JsonSerializer.Serialize(toolResult).Substring(0, Math.Min(200, JsonSerializer.Serialize(toolResult).Length))}...");
                }
                catch (Exception toolEx)
                {
                    Console.WriteLine($"   ?? Tool execution failed: {toolEx.Message}");
                }
            }
            else
            {
                Console.WriteLine("   No suitable test tool found");
            }

            Console.WriteLine("\n?? Step 6: Checking for available resources...");
            try
            {
                var resources = await mcpService.GetAvailableResourcesAsync();
                if (resources.Count > 0)
                {
                    Console.WriteLine($"   ? Found {resources.Count} resources");
                    foreach (var resource in resources.Take(3))
                    {
                        Console.WriteLine($"   - {JsonSerializer.Serialize(resource)}");
                    }
                }
                else
                {
                    Console.WriteLine("   ?? No resources available (this is normal if not implemented)");
                }
            }
            catch (Exception resourceEx)
            {
                Console.WriteLine($"   ?? Resources not supported: {resourceEx.Message}");
            }

            Console.WriteLine("\n?? Step 7: Testing educational request...");
            var testRequest = "What tools are available for course management?";
            Console.WriteLine($"   Request: {testRequest}");
            
            var response = await mcpService.ProcessEducationalRequestAsync(testRequest);
            Console.WriteLine($"? Response received ({response.Length} characters)");
            Console.WriteLine($"   Preview: {response.Substring(0, Math.Min(150, response.Length))}...");

            Console.WriteLine("\n?? All tests completed successfully!");
            Console.WriteLine("?? The MCP client is ready for use!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test failed");
            Console.WriteLine($"? Test failed: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
            }

            // Provide helpful troubleshooting tips
            Console.WriteLine("\n?? Troubleshooting tips:");
            Console.WriteLine("1. Ensure the MCP server project is built: dotnet build");
            Console.WriteLine("2. Check that .NET 8 SDK is installed: dotnet --version");
            Console.WriteLine("3. Verify the MCP server path in local.settings.json");
            Console.WriteLine("4. Try running the MCP server manually: dotnet run --project ../learnify.ai.mcp.server");
        }
        finally
        {
            mcpService?.Dispose();
            Console.WriteLine("\n?? Test completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}