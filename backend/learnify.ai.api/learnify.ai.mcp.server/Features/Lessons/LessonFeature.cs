using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Lessons.Services;

namespace Learnify.Mcp.Server.Features.Lessons;

/// <summary>
/// Lesson feature registration following vertical slice architecture
/// </summary>
public static class LessonFeature
{
    /// <summary>
    /// Register all lesson-related services
    /// Tools are automatically discovered by the MCP server using attributes
    /// </summary>
    public static IServiceCollection AddLessonFeature(this IServiceCollection services)
    {
        // Register services (tools are auto-discovered via attributes)
        services.AddScoped<LessonApiService>();
        
        return services;
    }
}