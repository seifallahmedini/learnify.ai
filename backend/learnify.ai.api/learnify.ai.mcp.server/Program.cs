using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Learnify.Mcp.Server.Features.Lessons;
using Learnify.Mcp.Server.Features.Courses;

namespace Learnify.Mcp.Server;

/// <summary>
/// Main program for the Learnify.ai MCP Server
/// Following the workspace's vertical slice architecture and .NET 8 patterns
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Configure logging to stderr for MCP protocol compliance
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            // Configure all logs to go to stderr
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Add HTTP client for API communication
        builder.Services.AddHttpClient();
        
        // Add lesson management features (vertical slice)
        builder.Services.AddLessonFeature();
        
        // Add course management features (vertical slice)
        builder.Services.AddCourseFeature();
        
        // Configure MCP server with tools from assembly
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        var host = builder.Build();
        
        await host.RunAsync();
    }
}