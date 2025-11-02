using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Courses;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(c => c.ParentCategoryId == null && c.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(c => c.ParentCategoryId == parentCategoryId && c.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(c => c.ParentCategoryId == categoryId && c.IsActive, cancellationToken);
    }

    public async Task<int> GetCourseCountAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .CountAsync(c => c.CategoryId == categoryId && c.IsPublished, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var hierarchy = new List<Category>();
        var currentCategory = await GetByIdAsync(categoryId, cancellationToken);
        
        while (currentCategory != null)
        {
            hierarchy.Insert(0, currentCategory);
            if (currentCategory.ParentCategoryId == null) break;
            currentCategory = await GetByIdAsync(currentCategory.ParentCategoryId.Value, cancellationToken);
        }
        
        return hierarchy;
    }
}
