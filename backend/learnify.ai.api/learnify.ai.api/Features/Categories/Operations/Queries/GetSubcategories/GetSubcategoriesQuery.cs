using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetSubcategoriesQuery(int ParentId) : IQuery<IEnumerable<CategorySummaryResponse>>;

public class GetSubcategoriesValidator : AbstractValidator<GetSubcategoriesQuery>
{
    public GetSubcategoriesValidator()
    {
        RuleFor(x => x.ParentId)
            .GreaterThan(0)
            .WithMessage("Parent category ID must be greater than 0");
    }
}

public class GetSubcategoriesHandler : IRequestHandler<GetSubcategoriesQuery, IEnumerable<CategorySummaryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public GetSubcategoriesHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CategorySummaryResponse>> Handle(GetSubcategoriesQuery request, CancellationToken cancellationToken)
    {
        // Verify parent category exists
        var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId, cancellationToken);
        if (parentCategory == null)
            throw new ArgumentException($"Parent category with ID {request.ParentId} not found");

        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var subcategories = allCategories
            .Where(c => c.ParentCategoryId == request.ParentId && c.IsActive)
            .OrderBy(c => c.Name);

        var result = new List<CategorySummaryResponse>();

        foreach (var category in subcategories)
        {
            var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == category.Id, cancellationToken);

            result.Add(new CategorySummaryResponse(
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

        return result;
    }
}
