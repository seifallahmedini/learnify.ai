using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Enrollments;
public interface IProgressRepository : IBaseRepository<Domain.Entities.Progress>
{
    Task<IEnumerable<Domain.Entities.Progress>> GetByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Progress>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Progress?> GetByEnrollmentAndLessonAsync(int enrollmentId, int lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Progress>> GetCompletedLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Progress>> GetIncompleteLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<int> GetCompletedLessonsCountAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalTimeSpentAsync(int enrollmentId, CancellationToken cancellationToken = default);
    Task<decimal> GetProgressPercentageAsync(int enrollmentId, CancellationToken cancellationToken = default);
}
