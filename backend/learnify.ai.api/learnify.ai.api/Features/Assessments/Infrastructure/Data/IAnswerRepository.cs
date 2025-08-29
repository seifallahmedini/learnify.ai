using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Assessments.Core.Models;

namespace learnify.ai.api.Features.Assessments.Infrastructure.Data;

public interface IAnswerRepository : IBaseRepository<Answer>
{
    Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Answer>> GetCorrectAnswersAsync(int questionId, CancellationToken cancellationToken = default);
    Task<Answer?> GetCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default);
    Task<Answer?> GetByOrderIndexAsync(int questionId, int orderIndex, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(int questionId, CancellationToken cancellationToken = default);
    Task<bool> HasCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default);
}