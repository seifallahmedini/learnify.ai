using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Courses.Core.Models;

namespace learnify.ai.api.Features.Courses.Infrastructure.Data;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<int> GetEnrollmentCountAsync(int courseId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetTotalStudentsAsync(int courseId, CancellationToken cancellationToken = default);
}