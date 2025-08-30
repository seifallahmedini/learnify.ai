using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record CreateCategoryCommand(
    string Name,
    string Description,
    string? IconUrl = null,
    int? ParentCategoryId = null,
    bool IsActive = true
) : ICommand<CategoryResponse>;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Category description is required")
            .MaximumLength(500)
            .WithMessage("Category description cannot exceed 500 characters");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500)
            .WithMessage("Icon URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.IconUrl));

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Parent category ID must be greater than 0 when specified");
    }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validate parent category exists if specified
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
                throw new ArgumentException($"Parent category with ID {request.ParentCategoryId} not found");
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl,
            ParentCategoryId = request.ParentCategoryId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.CreateAsync(category, cancellationToken);

        // Get parent category name if exists
        string? parentCategoryName = null;
        if (createdCategory.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(createdCategory.ParentCategoryId.Value, cancellationToken);
            parentCategoryName = parentCategory?.Name;
        }

        return new CategoryResponse(
            createdCategory.Id,
            createdCategory.Name,
            createdCategory.Description,
            createdCategory.IconUrl,
            createdCategory.ParentCategoryId,
            parentCategoryName,
            createdCategory.IsActive,
            0, // New category has no courses yet
            0, // New category has no subcategories yet
            createdCategory.CreatedAt,
            createdCategory.UpdatedAt
        );
    }
}