using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Courses.Core.Models;

namespace learnify.ai.api.Features.Courses.Infrastructure.Data;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<int> GetCourseCountAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoryHierarchyAsync(int categoryId, CancellationToken cancellationToken = default);
}