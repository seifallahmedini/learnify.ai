namespace learnify.ai.api.Features.Categories;

public record CreateCategoryRequest(
    string Name,
    string Description,
    string? IconUrl = null,
    int? ParentCategoryId = null,
    bool IsActive = true
);

public record UpdateCategoryRequest(
    string? Name = null,
    string? Description = null,
    string? IconUrl = null,
    int? ParentCategoryId = null,
    bool? IsActive = null
);

public record CategoryFilterRequest(
    bool? IsActive = null,
    int? ParentCategoryId = null,
    bool? RootOnly = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);

public record CategoryCoursesCountResponse(
    int CategoryId,
    string CategoryName,
    int DirectCourseCount,
    int TotalCourseCount,
    int SubcategoryCount
);
