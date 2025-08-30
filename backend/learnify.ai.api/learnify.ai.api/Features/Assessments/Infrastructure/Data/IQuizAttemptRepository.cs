using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public interface IQuizAttemptRepository : IBaseRepository<QuizAttempt>
{
    Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetByUserAndQuizAsync(int userId, int quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetLatestAttemptAsync(int userId, int quizId, CancellationToken cancellationToken = default);
    Task<int> GetAttemptCountAsync(int userId, int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetPassedAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetFailedAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetInProgressAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task<double> GetAverageScoreAsync(int quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetBestAttemptAsync(int userId, int quizId, CancellationToken cancellationToken = default);
}