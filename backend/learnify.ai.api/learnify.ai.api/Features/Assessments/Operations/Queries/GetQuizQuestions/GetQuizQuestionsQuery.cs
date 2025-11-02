using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
namespace learnify.ai.api.Features.Assessments;

public record GetQuizQuestionsQuery(
    int QuizId
) : IQuery<QuizQuestionsResponse>;

public class GetQuizQuestionsValidator : AbstractValidator<GetQuizQuestionsQuery>
{
    public GetQuizQuestionsValidator()
    {
        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class GetQuizQuestionsHandler : IRequestHandler<GetQuizQuestionsQuery, QuizQuestionsResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;

    public GetQuizQuestionsHandler(
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
    }

    public async Task<QuizQuestionsResponse> Handle(GetQuizQuestionsQuery request, CancellationToken cancellationToken)
    {
        // Verify quiz exists
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        // Get all questions for the quiz
        var questions = await _questionRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);
        var questionsList = questions.OrderBy(q => q.OrderIndex).ToList();

        // Build question summaries
        var questionSummaries = new List<QuestionSummaryResponse>();
        foreach (var question in questionsList)
        {
            var answerCount = (await _answerRepository.GetByQuestionIdAsync(question.Id, cancellationToken)).Count();
            
            questionSummaries.Add(new QuestionSummaryResponse(
                question.Id,
                question.QuizId,
                question.QuestionText,
                question.QuestionType,
                question.Points,
                question.OrderIndex,
                question.IsActive,
                answerCount
            ));
        }

        var totalPoints = questionsList.Sum(q => q.Points);

        return new QuizQuestionsResponse(
            quiz.Id,
            quiz.Title,
            questionSummaries,
            questionsList.Count,
            totalPoints
        );
    }
}