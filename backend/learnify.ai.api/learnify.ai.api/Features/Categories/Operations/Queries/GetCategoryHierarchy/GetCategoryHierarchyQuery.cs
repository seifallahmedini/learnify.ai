using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetCategoryHierarchyQuery(int Id) : IQuery<CategoryHierarchyResponse?>;

public class GetCategoryHierarchyValidator : AbstractValidator<GetCategoryHierarchyQuery>
{
    public GetCategoryHierarchyValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");
    }
}

public class GetCategoryHierarchyHandler : IRequestHandler<GetCategoryHierarchyQuery, CategoryHierarchyResponse?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCategoryHierarchyHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CategoryHierarchyResponse?> Handle(GetCategoryHierarchyQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return null;

        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoriesList = allCategories.ToList();

        return await BuildCategoryHierarchy(category, categoriesList, cancellationToken);
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
