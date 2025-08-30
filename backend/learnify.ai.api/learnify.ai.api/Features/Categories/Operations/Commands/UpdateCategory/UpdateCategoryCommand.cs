using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record UpdateCategoryCommand(
    int Id,
    string? Name = null,
    string? Description = null,
    string? IconUrl = null,
    int? ParentCategoryId = null,
    bool? IsActive = null
) : ICommand<CategoryResponse?>;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Category description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.IconUrl)
            .MaximumLength(500)
            .WithMessage("Icon URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.IconUrl));

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Parent category ID must be greater than 0 when specified");

        // Prevent self-reference
        RuleFor(x => x)
            .Must(x => x.ParentCategoryId != x.Id)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Category cannot be its own parent");
    }
}

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CategoryResponse?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return null;

        // Validate parent category exists if specified and changed
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId != category.ParentCategoryId)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
                throw new ArgumentException($"Parent category with ID {request.ParentCategoryId} not found");

            // Prevent circular reference
            if (await WouldCreateCircularReference(request.Id, request.ParentCategoryId.Value, cancellationToken))
                throw new ArgumentException("Cannot set parent category as it would create a circular reference");
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Name))
            category.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            category.Description = request.Description;

        if (request.IconUrl != null)
            category.IconUrl = request.IconUrl;

        if (request.ParentCategoryId.HasValue)
            category.ParentCategoryId = request.ParentCategoryId.Value;

        if (request.IsActive.HasValue)
            category.IsActive = request.IsActive.Value;

        category.UpdatedAt = DateTime.UtcNow;
        var updatedCategory = await _categoryRepository.UpdateAsync(category, cancellationToken);

        // Get parent category name if exists
        string? parentCategoryName = null;
        if (updatedCategory.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(updatedCategory.ParentCategoryId.Value, cancellationToken);
            parentCategoryName = parentCategory?.Name;
        }

        // Get counts
        var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == updatedCategory.Id, cancellationToken);
        var subcategoryCount = await _categoryRepository.CountAsync(c => c.ParentCategoryId == updatedCategory.Id, cancellationToken);

        return new CategoryResponse(
            updatedCategory.Id,
            updatedCategory.Name,
            updatedCategory.Description,
            updatedCategory.IconUrl,
            updatedCategory.ParentCategoryId,
            parentCategoryName,
            updatedCategory.IsActive,
            courseCount,
            subcategoryCount,
            updatedCategory.CreatedAt,
            updatedCategory.UpdatedAt
        );
    }

    private async Task<bool> WouldCreateCircularReference(int categoryId, int parentCategoryId, CancellationToken cancellationToken)
    {
        var currentCategoryId = parentCategoryId;
        var checkedCategories = new HashSet<int> { categoryId };

        while (currentCategoryId != 0)
        {
            if (checkedCategories.Contains(currentCategoryId))
                return true;

            checkedCategories.Add(currentCategoryId);
            var parentCategory = await _categoryRepository.GetByIdAsync(currentCategoryId, cancellationToken);
            
            if (parentCategory?.ParentCategoryId == null)
                break;

            currentCategoryId = parentCategory.ParentCategoryId.Value;
        }

        return false;
    }
}