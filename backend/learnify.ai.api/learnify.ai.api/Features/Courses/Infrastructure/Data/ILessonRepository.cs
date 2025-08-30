using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Courses;

public interface ILessonRepository : IBaseRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lesson>> GetPublishedLessonsAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lesson>> GetFreeLessonsAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Lesson?> GetNextLessonAsync(int courseId, int currentOrderIndex, CancellationToken cancellationToken = default);
    Task<Lesson?> GetPreviousLessonAsync(int courseId, int currentOrderIndex, CancellationToken cancellationToken = default);
    Task<int> GetTotalDurationAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetLessonCountAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetTotalLessonCountByCourseAsync(int courseId, CancellationToken cancellationToken);
}