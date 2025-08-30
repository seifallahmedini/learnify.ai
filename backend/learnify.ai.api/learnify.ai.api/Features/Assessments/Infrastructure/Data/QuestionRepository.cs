using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Question>> GetActiveQuestionsAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(q => q.QuizId == quizId && q.IsActive)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetByOrderIndexAsync(int quizId, int orderIndex, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(q => q.QuizId == quizId && q.OrderIndex == orderIndex, cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexAsync(int quizId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await _dbSet
            .Where(q => q.QuizId == quizId)
            .MaxAsync(q => (int?)q.OrderIndex, cancellationToken);
        
        return maxOrder ?? 0;
    }

    public async Task<IEnumerable<Question>> GetByTypeAsync(int quizId, QuestionType questionType, CancellationToken cancellationToken = default)
    {
        return await FindAsync(q => q.QuizId == quizId && q.QuestionType == questionType && q.IsActive, cancellationToken);
    }

    public async Task<int> GetTotalPointsAsync(int quizId, CancellationToken cancellationToken = default)
    {
        var totalPoints = await _dbSet
            .Where(q => q.QuizId == quizId && q.IsActive)
            .SumAsync(q => q.Points, cancellationToken);
        
        return totalPoints;
    }
}