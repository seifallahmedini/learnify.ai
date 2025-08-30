using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetCategoryByIdQuery(int Id) : IQuery<CategoryResponse?>;

public class GetCategoryByIdValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");
    }
}

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CategoryResponse?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return null;

        // Get parent category name if exists
        string? parentCategoryName = null;
        if (category.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(category.ParentCategoryId.Value, cancellationToken);
            parentCategoryName = parentCategory?.Name;
        }

        // Get course count
        var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == category.Id, cancellationToken);

        // Get subcategory count
        var subcategoryCount = await _categoryRepository.CountAsync(c => c.ParentCategoryId == category.Id, cancellationToken);

        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.IconUrl,
            category.ParentCategoryId,
            parentCategoryName,
            category.IsActive,
            courseCount,
            subcategoryCount,
            category.CreatedAt,
            category.UpdatedAt
        );
    }
}