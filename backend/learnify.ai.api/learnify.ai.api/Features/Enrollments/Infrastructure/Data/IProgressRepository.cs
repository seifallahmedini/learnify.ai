using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Enrollments.Core.Models;

namespace learnify.ai.api.Features.Enrollments.Infrastructure.Data;

public interface IProgressRepository : IBaseRepository<Progress>
{
    Task<IEnumerable<Progress>> GetByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<Progress?> GetByEnrollmentAndLessonAsync(int enrollmentId, int lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetCompletedLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetIncompleteLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<int> GetCompletedLessonsCountAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalTimeSpentAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<decimal> GetProgressPercentageAsync(int enrollmentId, CancellationToken cancellationToken = default);
}