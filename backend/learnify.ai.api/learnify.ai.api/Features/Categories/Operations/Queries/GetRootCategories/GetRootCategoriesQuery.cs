using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetRootCategoriesQuery(
    bool? IsActive = null,
    string? SearchTerm = null
) : IQuery<IEnumerable<CategoryHierarchyResponse>>;

public class GetRootCategoriesValidator : AbstractValidator<GetRootCategoriesQuery>
{
    public GetRootCategoriesValidator()
    {
        // No specific validation needed for this query
    }
}

public class GetRootCategoriesHandler : IRequestHandler<GetRootCategoriesQuery, IEnumerable<CategoryHierarchyResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public GetRootCategoriesHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CategoryHierarchyResponse>> Handle(GetRootCategoriesQuery request, CancellationToken cancellationToken)
    {
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoriesList = allCategories.ToList();

        // Filter root categories
        var rootCategories = categoriesList.Where(c => c.ParentCategoryId == null);

        // Apply filters
        if (request.IsActive.HasValue)
        {
            rootCategories = rootCategories.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            rootCategories = rootCategories.Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        var result = new List<CategoryHierarchyResponse>();

        foreach (var category in rootCategories.OrderBy(c => c.Name))
        {
            var hierarchyResponse = await BuildCategoryHierarchy(category, categoriesList, cancellationToken);
            result.Add(hierarchyResponse);
        }

        return result;
    }

    private async Task<CategoryHierarchyResponse> BuildCategoryHierarchy(Category category, List<Category> allCategories, CancellationToken cancellationToken)
    {
        var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == category.Id, cancellationToken);
        var children = allCategories.Where(c => c.ParentCategoryId == category.Id).OrderBy(c => c.Name);

        var childrenHierarchy = new List<CategoryHierarchyResponse>();
        foreach (var child in children)
        {
            var childHierarchy = await BuildCategoryHierarchy(child, allCategories, cancellationToken);
            childrenHierarchy.Add(childHierarchy);
        }

        return new CategoryHierarchyResponse(
            category.Id,
            category.Name,
            category.Description,
            category.IconUrl,
            category.IsActive,
            courseCount,
            childrenHierarchy
        );
    }
}
