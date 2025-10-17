using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Categories.Models;

namespace Learnify.Mcp.Server.Features.Categories.Services;

/// <summary>
/// API service for category management operations
/// </summary>
[McpServerToolType]
public class CategoryApiService : BaseApiService
{
    public CategoryApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CategoryApiService> logger)
        : base(httpClient, configuration, logger, "CategoryApiService")
    {
    }

    #region Category CRUD Operations

    /// <summary>
    /// Get all categories with optional filtering
    /// </summary>
    [McpServerTool, Description("Get all categories with optional filtering and pagination")]
    public async Task<string> GetCategoriesAsync(
        [Description("Active status filter (optional)")] bool? isActive = null,
        [Description("Parent category ID filter (optional)")] int? parentCategoryId = null,
        [Description("Show only root categories (optional)")] bool? rootOnly = null,
        [Description("Search term for name/description (optional)")] string? searchTerm = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting categories with filters - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var queryParams = new List<string>();
            if (isActive.HasValue) queryParams.Add($"isActive={isActive}");
            if (parentCategoryId.HasValue) queryParams.Add($"parentCategoryId={parentCategoryId}");
            if (rootOnly.HasValue) queryParams.Add($"rootOnly={rootOnly}");
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var categories = await GetAsync<CategoryListResponse>($"/api/categories{queryString}", cancellationToken);

            if (categories == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No categories found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = categories,
                message = "Categories retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get category by ID with full details
    /// </summary>
    [McpServerTool, Description("Get category details by ID")]
    public async Task<string> GetCategoryAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting category with ID: {CategoryId}", categoryId);
            var category = await GetAsync<CategoryModel>($"/api/categories/{categoryId}", cancellationToken);

            if (category == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = category,
                message = "Category retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [McpServerTool, Description("Create a new category")]
    public async Task<string> CreateCategoryAsync(
        [Description("Category name")] string name,
        [Description("Category description")] string description,
        [Description("Category icon URL (optional)")] string? iconUrl = null,
        [Description("Parent category ID (optional)")] int? parentCategoryId = null,
        [Description("Whether the category is active")] bool isActive = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating category: {Name}", name);

            var request = new CreateCategoryRequest(
                name, description, iconUrl, parentCategoryId, isActive);

            var createdCategory = await PostAsync<CategoryModel>("/api/categories", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = createdCategory,
                message = "Category created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category {Name}", name);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [McpServerTool, Description("Update category details")]
    public async Task<string> UpdateCategoryAsync(
        [Description("The category ID")] int categoryId,
        [Description("Category name (optional)")] string? name = null,
        [Description("Category description (optional)")] string? description = null,
        [Description("Category icon URL (optional)")] string? iconUrl = null,
        [Description("Parent category ID (optional)")] int? parentCategoryId = null,
        [Description("Whether the category is active (optional)")] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating category with ID: {CategoryId}", categoryId);

            var request = new UpdateCategoryRequest(
                name, description, iconUrl, parentCategoryId, isActive);

            var updatedCategory = await PutAsync<CategoryModel>($"/api/categories/{categoryId}", request, cancellationToken);

            if (updatedCategory == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedCategory,
                message = "Category updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete a category permanently
    /// </summary>
    [McpServerTool, Description("Delete a category permanently")]
    public async Task<string> DeleteCategoryAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", categoryId);
            var success = await DeleteAsync($"/api/categories/{categoryId}", cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success,
                message = success ? "Category deleted successfully" : "Failed to delete category"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Category Activation Operations

    /// <summary>
    /// Activate a category
    /// </summary>
    [McpServerTool, Description("Activate a category to make it visible")]
    public async Task<string> ActivateCategoryAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Activating category with ID: {CategoryId}", categoryId);
            var activatedCategory = await PutAsync<CategoryModel>($"/api/categories/{categoryId}/activate", null, cancellationToken);

            if (activatedCategory == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = activatedCategory,
                message = "Category activated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Deactivate a category
    /// </summary>
    [McpServerTool, Description("Deactivate a category to hide it")]
    public async Task<string> DeactivateCategoryAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deactivating category with ID: {CategoryId}", categoryId);
            var deactivatedCategory = await PutAsync<CategoryModel>($"/api/categories/{categoryId}/deactivate", null, cancellationToken);

            if (deactivatedCategory == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = deactivatedCategory,
                message = "Category deactivated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Category Hierarchy Operations

    /// <summary>
    /// Get category tree structure
    /// </summary>
    [McpServerTool, Description("Get complete category hierarchy as a tree structure")]
    public async Task<string> GetCategoryTreeAsync(
        [Description("Include only active categories (optional)")] bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting category tree");
            
            var queryString = activeOnly.HasValue ? $"?activeOnly={activeOnly}" : "";
            var categoryTree = await GetAsync<CategoryTreeResponse>($"/api/categories/tree{queryString}", cancellationToken);

            if (categoryTree == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No category tree found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = categoryTree,
                message = "Category tree retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category tree");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get subcategories of a specific category
    /// </summary>
    [McpServerTool, Description("Get all subcategories of a specific category")]
    public async Task<string> GetSubcategoriesAsync(
        [Description("The parent category ID")] int categoryId,
        [Description("Include only active subcategories (optional)")] bool? activeOnly = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting subcategories for category ID: {CategoryId}", categoryId);
            
            var queryParams = new List<string>();
            if (activeOnly.HasValue) queryParams.Add($"activeOnly={activeOnly}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var subcategories = await GetAsync<CategoryListResponse>($"/api/categories/{categoryId}/subcategories{queryString}", cancellationToken);

            if (subcategories == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No subcategories found for category ID {categoryId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = subcategories,
                message = "Subcategories retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories for category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Move category to a different parent
    /// </summary>
    [McpServerTool, Description("Move a category to a different parent or make it a root category")]
    public async Task<string> MoveCategoryAsync(
        [Description("The category ID to move")] int categoryId,
        [Description("New parent category ID (null for root category)")] int? newParentCategoryId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Moving category {CategoryId} to parent {NewParentId}", categoryId, newParentCategoryId);

            var request = new MoveCategoryRequest(newParentCategoryId);
            var movedCategory = await PutAsync<CategoryModel>($"/api/categories/{categoryId}/move", request, cancellationToken);

            if (movedCategory == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = movedCategory,
                message = "Category moved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Category Content Operations

    /// <summary>
    /// Get courses in a specific category
    /// </summary>
    [McpServerTool, Description("Get all courses in a specific category")]
    public async Task<string> GetCategoryCoursesAsync(
        [Description("The category ID")] int categoryId,
        [Description("Include only published courses (optional)")] bool? publishedOnly = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting courses for category ID: {CategoryId}", categoryId);
            
            var queryParams = new List<string>();
            if (publishedOnly.HasValue) queryParams.Add($"publishedOnly={publishedOnly}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var categoryCourses = await GetAsync<CategoryCoursesResponse>($"/api/categories/{categoryId}/courses{queryString}", cancellationToken);

            if (categoryCourses == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No courses found for category ID {categoryId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = categoryCourses,
                message = "Category courses retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get category statistics
    /// </summary>
    [McpServerTool, Description("Get comprehensive statistics for a category")]
    public async Task<string> GetCategoryStatsAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting statistics for category ID: {CategoryId}", categoryId);
            var stats = await GetAsync<CategoryStatsModel>($"/api/categories/{categoryId}/stats", cancellationToken);

            if (stats == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No statistics found for category ID {categoryId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = stats,
                message = "Category statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for category {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if category exists
    /// </summary>
    [McpServerTool, Description("Check if a category exists")]
    public async Task<string> CheckCategoryExistsAsync(
        [Description("The category ID to check")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking if category exists: {CategoryId}", categoryId);
            var category = await GetAsync<CategoryModel>($"/api/categories/{categoryId}", cancellationToken);
            var exists = category != null;

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                message = exists ? "Category exists" : "Category does not exist"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking category existence {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get category summary (basic info only)
    /// </summary>
    [McpServerTool, Description("Get category summary (basic information only)")]
    public async Task<string> GetCategorySummaryAsync(
        [Description("The category ID")] int categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting category summary for ID: {CategoryId}", categoryId);
            var category = await GetAsync<CategoryModel>($"/api/categories/{categoryId}", cancellationToken);

            if (category == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Category with ID {categoryId} not found"
                });
            }

            var summary = new CategorySummaryModel(
                category.Id,
                category.Name,
                category.Description,
                category.IconUrl,
                category.ParentCategoryId,
                category.ParentCategoryName,
                category.IsActive,
                category.CourseCount,
                category.CreatedAt
            );

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Category summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category summary {CategoryId}", categoryId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get root categories only
    /// </summary>
    [McpServerTool, Description("Get all root categories (categories without parents)")]
    public async Task<string> GetRootCategoriesAsync(
        [Description("Include only active categories (optional)")] bool? activeOnly = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting root categories");
            
            var queryParams = new List<string>();
            queryParams.Add("rootOnly=true");
            if (activeOnly.HasValue) queryParams.Add($"isActive={activeOnly}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = "?" + string.Join("&", queryParams);
            var rootCategories = await GetAsync<CategoryListResponse>($"/api/categories{queryString}", cancellationToken);

            if (rootCategories == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No root categories found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = rootCategories,
                message = "Root categories retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root categories");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion
}