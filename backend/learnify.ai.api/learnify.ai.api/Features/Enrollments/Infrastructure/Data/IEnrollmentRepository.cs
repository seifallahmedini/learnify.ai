using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Enrollments.Core.Models;

namespace learnify.ai.api.Features.Enrollments.Infrastructure.Data;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByStatusAsync(EnrollmentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetEnrollmentCountByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId, CancellationToken cancellationToken = default);
}