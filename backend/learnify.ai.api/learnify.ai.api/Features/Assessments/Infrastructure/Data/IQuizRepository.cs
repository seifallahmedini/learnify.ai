using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Assessments.Core.Models;

namespace learnify.ai.api.Features.Assessments.Infrastructure.Data;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetActiveQuizzesAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetTotalPointsAsync(int quizId, CancellationToken cancellationToken = default);
    Task<int> GetQuestionCountAsync(int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetQuizzesByInstructorAsync(int instructorId, CancellationToken cancellationToken = default);
}