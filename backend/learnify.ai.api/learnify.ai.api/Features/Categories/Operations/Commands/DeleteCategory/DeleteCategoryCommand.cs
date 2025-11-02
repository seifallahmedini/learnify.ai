using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record DeleteCategoryCommand(int Id) : ICommand<bool>;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");
    }
}

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return false;

        // Check if category has courses
        var courseCount = await _courseRepository.CountAsync(c => c.CategoryId == request.Id, cancellationToken);
        if (courseCount > 0)
            throw new InvalidOperationException($"Cannot delete category because it contains {courseCount} courses. Please move or delete the courses first.");

        // Check if category has subcategories
        var subcategoryCount = await _categoryRepository.CountAsync(c => c.ParentCategoryId == request.Id, cancellationToken);
        if (subcategoryCount > 0)
            throw new InvalidOperationException($"Cannot delete category because it contains {subcategoryCount} subcategories. Please move or delete the subcategories first.");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        return true;
    }
}
