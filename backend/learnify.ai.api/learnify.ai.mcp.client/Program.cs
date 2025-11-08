using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services;
using Learnify.Mcp.Server.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Configure Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register Learnify MCP Server (in-process) - includes InProcessMcpToolInvoker
        services.AddLearnifyMcpServer();
        
        // Core MCP client services (now using in-process communication)
        services.AddScoped<LearnifyMcpService>();
        services.AddScoped<McpToolsService>();
        services.AddScoped<AiAgentService>();
        
        // Add HTTP client
        services.AddHttpClient();

        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddApplicationInsights();
        });
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration sources
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .Build();

host.Run();