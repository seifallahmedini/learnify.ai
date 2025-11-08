using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Lessons;
using Learnify.Mcp.Server.Features.Courses;
using Learnify.Mcp.Server.Features.Categories;
using Learnify.Mcp.Server.Features.Quizzes;
using Learnify.Mcp.Server.Features.Answers;
using Learnify.Mcp.Server.Services;

namespace Learnify.Mcp.Server.Extensions;

/// <summary>
/// Extension methods for registering Learnify MCP Server services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all Learnify MCP Server features for in-process tool execution
    /// </summary>
    public static IServiceCollection AddLearnifyMcpServer(this IServiceCollection services)
    {
        // Add HTTP client for API communication
        services.AddHttpClient();
        
        // Add lesson management features (vertical slice)
        services.AddLessonFeature();
        
        // Add course management features (vertical slice)
        services.AddCourseFeature();
        
        // Add category management features (vertical slice)
        services.AddCategoryFeature();
        
        // Add quiz management features (vertical slice)
        services.AddQuizFeature();
        
        // Add answer management features (vertical slice)
        services.AddAnswerFeature();
        
        // Register the in-process tool invoker
        services.AddScoped<InProcessMcpToolInvoker>();
        
        return services;
    }
}