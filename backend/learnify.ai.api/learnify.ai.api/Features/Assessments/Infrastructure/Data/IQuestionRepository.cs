using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public interface IQuestionRepository : IBaseRepository<Question>
{
    Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Question>> GetActiveQuestionsAsync(int quizId, CancellationToken cancellationToken = default);
    Task<Question?> GetByOrderIndexAsync(int quizId, int orderIndex, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(int quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Question>> GetByTypeAsync(int quizId, QuestionType questionType, CancellationToken cancellationToken = default);
    Task<int> GetTotalPointsAsync(int quizId, CancellationToken cancellationToken = default);
}