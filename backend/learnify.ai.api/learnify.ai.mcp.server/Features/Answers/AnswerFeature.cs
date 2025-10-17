using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Answers.Services;

namespace Learnify.Mcp.Server.Features.Answers;

/// <summary>
/// Answer feature registration following vertical slice architecture
/// </summary>
public static class AnswerFeature
{
    /// <summary>
    /// Register all answer-related services
    /// Tools are automatically discovered by the MCP server using attributes
    /// </summary>
    public static IServiceCollection AddAnswerFeature(this IServiceCollection services)
    {
        // Register services (tools are auto-discovered via attributes)
        services.AddScoped<AnswerApiService>();
        
        return services;
    }
}