using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Quizzes.Services;

namespace Learnify.Mcp.Server.Features.Quizzes;

/// <summary>
/// Quiz feature registration following vertical slice architecture
/// </summary>
public static class QuizFeature
{
    /// <summary>
    /// Register all quiz-related services
    /// Tools are automatically discovered by the MCP server using attributes
    /// </summary>
    public static IServiceCollection AddQuizFeature(this IServiceCollection services)
    {
        // Register services (tools are auto-discovered via attributes)
        services.AddScoped<QuizApiService>();
        
        return services;
    }
}