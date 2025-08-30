using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetActiveQuizzesAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetTotalPointsAsync(int quizId, CancellationToken cancellationToken = default);
    Task<int> GetQuestionCountAsync(int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetQuizzesByInstructorAsync(int instructorId, CancellationToken cancellationToken = default);
}