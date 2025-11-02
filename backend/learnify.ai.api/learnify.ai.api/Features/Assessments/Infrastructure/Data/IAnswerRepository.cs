using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Assessments;

public interface IAnswerRepository : IBaseRepository<Answer>
{
    Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Answer>> GetCorrectAnswersAsync(int questionId, CancellationToken cancellationToken = default);
    Task<Answer?> GetCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default);
    Task<Answer?> GetByOrderIndexAsync(int questionId, int orderIndex, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(int questionId, CancellationToken cancellationToken = default);
    Task<bool> HasCorrectAnswerAsync(int questionId, CancellationToken cancellationToken = default);
}
