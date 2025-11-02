using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Categories;

public record GetCategoryBreadcrumbQuery(int Id) : IQuery<CategoryBreadcrumbResponse?>;

public class GetCategoryBreadcrumbValidator : AbstractValidator<GetCategoryBreadcrumbQuery>
{
    public GetCategoryBreadcrumbValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");
    }
}

public class GetCategoryBreadcrumbHandler : IRequestHandler<GetCategoryBreadcrumbQuery, CategoryBreadcrumbResponse?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryBreadcrumbHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryBreadcrumbResponse?> Handle(GetCategoryBreadcrumbQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return null;

        var breadcrumbs = new List<CategoryBreadcrumbItem>();
        var currentCategory = category;

        // Build breadcrumb from current category to root
        while (currentCategory != null)
        {
            breadcrumbs.Insert(0, new CategoryBreadcrumbItem(
                currentCategory.Id,
                currentCategory.Name,
                currentCategory.IconUrl
            ));

            if (currentCategory.ParentCategoryId.HasValue)
            {
                currentCategory = await _categoryRepository.GetByIdAsync(currentCategory.ParentCategoryId.Value, cancellationToken);
            }
            else
            {
                currentCategory = null;
            }
        }

        return new CategoryBreadcrumbResponse(breadcrumbs);
    }
}
