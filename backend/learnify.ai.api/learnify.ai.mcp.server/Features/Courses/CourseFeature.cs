using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Courses.Services;

namespace Learnify.Mcp.Server.Features.Courses;

/// <summary>
/// Course feature registration following vertical slice architecture
/// </summary>
public static class CourseFeature
{
    /// <summary>
    /// Register all course-related services
    /// Tools are automatically discovered by the MCP server using attributes
    /// </summary>
    public static IServiceCollection AddCourseFeature(this IServiceCollection services)
    {
        // Register services (tools are auto-discovered via attributes)
        services.AddScoped<CourseApiService>();
        
        return services;
    }
}