using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public class AnswerRepository : BaseRepository<Answer>, IAnswerRepository
{
    public AnswerRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Answer>> GetCorrectAnswersAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(a => a.QuestionId == questionId && a.IsCorrect, cancellationToken);
    }

    public async Task<Answer?> GetCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsCorrect, cancellationToken);
    }

    public async Task<Answer?> GetByOrderIndexAsync(int questionId, int orderIndex, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(a => a.QuestionId == questionId && a.OrderIndex == orderIndex, cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexAsync(int questionId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await _dbSet
            .Where(a => a.QuestionId == questionId)
            .MaxAsync(a => (int?)a.OrderIndex, cancellationToken);
        
        return maxOrder ?? 0;
    }

    public async Task<bool> HasCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(a => a.QuestionId == questionId && a.IsCorrect, cancellationToken);
    }
}