using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Courses;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasSubCategoriesAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<int> GetCourseCountAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoryHierarchyAsync(int categoryId, CancellationToken cancellationToken = default);
}
