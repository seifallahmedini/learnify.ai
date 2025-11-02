using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Assessments;

public class QuizAttemptRepository : BaseRepository<QuizAttempt>, IQuizAttemptRepository
{
    public QuizAttemptRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(qa => qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(qa => qa.UserId == userId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetByUserAndQuizAsync(int userId, int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuizAttempt?> GetLatestAttemptAsync(int userId, int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetAttemptCountAsync(int userId, int quizId, CancellationToken cancellationToken = default)
    {
        return await CountAsync(qa => qa.UserId == userId && qa.QuizId == quizId, cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetPassedAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(qa => qa.UserId == userId && qa.IsPassed, cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetFailedAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(qa => qa.UserId == userId && !qa.IsPassed && qa.CompletedAt != null, cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetInProgressAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(qa => qa.UserId == userId && qa.CompletedAt == null, cancellationToken);
    }

    public async Task<double> GetAverageScoreAsync(int quizId, CancellationToken cancellationToken = default)
    {
        var attempts = await _dbSet
            .Where(qa => qa.QuizId == quizId && qa.CompletedAt != null)
            .ToListAsync(cancellationToken);

        if (!attempts.Any()) return 0;

        return attempts.Average(qa => qa.GetScorePercentage());
    }

    public async Task<QuizAttempt?> GetBestAttemptAsync(int userId, int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(qa => qa.UserId == userId && qa.QuizId == quizId && qa.CompletedAt != null)
            .OrderByDescending(qa => qa.Score)
            .ThenByDescending(qa => qa.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
