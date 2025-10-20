# Test MCP Connection

## Simple Console Test

You can test the MCP connection by running this simple test:

```csharp
// Create a simple test in a separate file or use the existing TestConsole.cs

// First, ensure the MCP server is built:
cd "../learnify.ai.mcp.server"
dotnet build --configuration Release

// Then test the connection by creating a minimal test:
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        ["McpServer:Path"] = @"C:\Users\LENOVO\Desktop\100 Days Challenge\learnify.ai\backend\learnify.ai.api\learnify.ai.mcp.server",
        ["McpServer:Executable"] = "dotnet"
    })
    .Build();

var services = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
    .AddSingleton<IConfiguration>(configuration)
    .AddHttpClient()
    .AddScoped<LearnifyMcpService>()
    .BuildServiceProvider();

var mcpService = services.GetRequiredService<LearnifyMcpService>();

try
{
    await mcpService.InitializeAsync();
    var tools = await mcpService.GetAvailableToolsAsync();
    Console.WriteLine($"Found {tools.Count} tools: {string.Join(", ", tools)}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    mcpService.Dispose();
}
```

## Alternative: Use Azure Functions

You can also test through the Azure Functions endpoints:

1. Start the function app:
```bash
func start
```

2. Test endpoints:
```bash
# Health check
curl http://localhost:7071/api/Health

# Get available tools
curl http://localhost:7071/api/GetAvailableTools
```

## Configuration Notes

Make sure your `local.settings.json` has the correct path:

```json
{
  "Values": {
    "McpServer:Path": "C:\\Users\\LENOVO\\Desktop\\100 Days Challenge\\learnify.ai\\backend\\learnify.ai.api\\learnify.ai.mcp.server"
  }
}
```

## Expected Results

When working correctly, you should see:
- MCP server process starts successfully
- Tools are discovered (lessons, courses, categories, quizzes, answers)
- Health status shows "Connected"
- Tool count should be > 0