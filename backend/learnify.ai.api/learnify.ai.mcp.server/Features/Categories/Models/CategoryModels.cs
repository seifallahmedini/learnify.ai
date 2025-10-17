using System.ComponentModel.DataAnnotations;

namespace Learnify.Mcp.Server.Features.Categories.Models;

#region Category Models

/// <summary>
/// Category model representing the main category entity
/// </summary>
public record CategoryModel(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    int? ParentCategoryId,
    string? ParentCategoryName,
    bool IsActive,
    int CourseCount,
    int SubcategoryCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Category summary model for lightweight operations
/// </summary>
public record CategorySummaryModel(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    int? ParentCategoryId,
    string? ParentCategoryName,
    bool IsActive,
    int CourseCount,
    DateTime CreatedAt
);

/// <summary>
/// Category list response model with pagination
/// </summary>
public record CategoryListResponse(
    IEnumerable<CategorySummaryModel> Categories,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Category hierarchy model for tree structure
/// </summary>
public record CategoryHierarchyModel(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    bool IsActive,
    int CourseCount,
    IEnumerable<CategoryHierarchyModel> Subcategories
);

/// <summary>
/// Category statistics model
/// </summary>
public record CategoryStatsModel(
    int CategoryId,
    string CategoryName,
    int TotalCourses,
    int PublishedCourses,
    int UnpublishedCourses,
    int TotalSubcategories,
    int ActiveSubcategories,
    int TotalEnrollments,
    decimal TotalRevenue,
    DateTime LastUpdated
);

#endregion

#region Request Models

/// <summary>
/// Request model for creating a new category
/// </summary>
public record CreateCategoryRequest(
    [Required] string Name,
    [Required] string Description,
    string? IconUrl = null,
    int? ParentCategoryId = null,
    bool IsActive = true
);

/// <summary>
/// Request model for updating an existing category
/// </summary>
public record UpdateCategoryRequest(
    string? Name,
    string? Description,
    string? IconUrl,
    int? ParentCategoryId,
    bool? IsActive
);

/// <summary>
/// Request model for filtering categories
/// </summary>
public record CategoryFilterRequest(
    bool? IsActive = null,
    int? ParentCategoryId = null,
    bool? RootOnly = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);

/// <summary>
/// Request model for category activation/deactivation
/// </summary>
public record CategoryActivationRequest(
    bool IsActive
);

/// <summary>
/// Request model for moving category to different parent
/// </summary>
public record MoveCategoryRequest(
    int? NewParentCategoryId
);

#endregion

#region Response Models

/// <summary>
/// Category courses response model
/// </summary>
public record CategoryCoursesResponse(
    int CategoryId,
    string CategoryName,
    IEnumerable<CategoryCourseModel> Courses,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Course model within category context
/// </summary>
public record CategoryCourseModel(
    int Id,
    string Title,
    string Description,
    decimal Price,
    decimal? DiscountPrice,
    bool IsPublished,
    bool IsFeatured,
    int EnrollmentCount,
    DateTime CreatedAt
);

/// <summary>
/// Category tree response for hierarchical display
/// </summary>
public record CategoryTreeResponse(
    IEnumerable<CategoryHierarchyModel> RootCategories,
    int TotalCategories,
    int MaxDepth
);

#endregion