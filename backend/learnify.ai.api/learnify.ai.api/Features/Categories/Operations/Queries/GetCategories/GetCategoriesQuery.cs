using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetCategoriesQuery(
    bool? IsActive = null,
    int? ParentCategoryId = null,
    bool? RootOnly = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<CategoryListResponse>;

public class GetCategoriesValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Parent category ID must be greater than 0 when specified");
    }
}

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, CategoryListResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CategoryListResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoriesList = categories.ToList();

        // Apply filters
        var filteredCategories = categoriesList.AsQueryable();

        if (request.IsActive.HasValue)
        {
            filteredCategories = filteredCategories.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (request.RootOnly.HasValue && request.RootOnly.Value)
        {
            filteredCategories = filteredCategories.Where(c => c.ParentCategoryId == null);
        }
        else if (request.ParentCategoryId.HasValue)
        {
            filteredCategories = filteredCategories.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredCategories = filteredCategories.Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        var totalCount = filteredCategories.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedCategories = filteredCategories
            .OrderBy(c => c.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var categorySummaries = new List<CategorySummaryResponse>();

        foreach (var category in paginatedCategories)
        {
            var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == category.Id, cancellationToken);

            categorySummaries.Add(new CategorySummaryResponse(
                category.Id,
                category.Name,
                category.Description,
                category.IconUrl,
                category.ParentCategoryId,
                category.IsActive,
                courseCount,
                category.CreatedAt
            ));
        }

        return new CategoryListResponse(
            categorySummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}