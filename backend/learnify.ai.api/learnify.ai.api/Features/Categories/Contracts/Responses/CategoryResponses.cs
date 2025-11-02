namespace learnify.ai.api.Features.Categories;

public record CategoryResponse(
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
)
{
    public bool IsRootCategory => ParentCategoryId == null;
}

public record CategorySummaryResponse(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    int? ParentCategoryId,
    bool IsActive,
    int CourseCount,
    DateTime CreatedAt
);

public record CategoryListResponse(
    IEnumerable<CategorySummaryResponse> Categories,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record CategoryHierarchyResponse(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    bool IsActive,
    int CourseCount,
    IEnumerable<CategoryHierarchyResponse> Children
);

public record CategoryBreadcrumbResponse(
    IEnumerable<CategoryBreadcrumbItem> Breadcrumbs
);

public record CategoryBreadcrumbItem(
    int Id,
    string Name,
    string? IconUrl
);

public record CategoryAnalyticsResponse(
    int CategoryId,
    string CategoryName,
    int DirectCourseCount,
    int TotalCourseCount,
    int SubcategoryCount,
    int TotalEnrollments,
    decimal AverageRating,
    DateTime AnalyticsDate
);

public record TrendingCategoryResponse(
    int Id,
    string Name,
    string Description,
    string? IconUrl,
    int CourseCount,
    int RecentEnrollments,
    decimal GrowthRate,
    decimal AverageRating
);
