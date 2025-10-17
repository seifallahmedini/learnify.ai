using Microsoft.Extensions.DependencyInjection;
using Learnify.Mcp.Server.Features.Categories.Services;

namespace Learnify.Mcp.Server.Features.Categories;

/// <summary>
/// Category feature registration following vertical slice architecture
/// </summary>
public static class CategoryFeature
{
    /// <summary>
    /// Register all category-related services
    /// Tools are automatically discovered by the MCP server using attributes
    /// </summary>
    public static IServiceCollection AddCategoryFeature(this IServiceCollection services)
    {
        // Register services (tools are auto-discovered via attributes)
        services.AddScoped<CategoryApiService>();
        
        return services;
    }
}