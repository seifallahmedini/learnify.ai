using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Assessments;

public record GetQuestionAnswersQuery(
    int QuestionId
) : IQuery<QuestionAnswersResponse>;

public class GetQuestionAnswersValidator : AbstractValidator<GetQuestionAnswersQuery>
{
    public GetQuestionAnswersValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");
    }
}

public class GetQuestionAnswersHandler : IRequestHandler<GetQuestionAnswersQuery, QuestionAnswersResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public GetQuestionAnswersHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<QuestionAnswersResponse> Handle(GetQuestionAnswersQuery request, CancellationToken cancellationToken)
    {
        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            throw new ArgumentException($"Question with ID {request.QuestionId} not found");

        // Get all answers for the question
        var answers = await _answerRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
        var answersList = answers.OrderBy(a => a.OrderIndex).ToList();

        // Build answer summaries
        var answerSummaries = answersList.Select(a => new AnswerSummaryResponse(
            a.Id,
            a.QuestionId,
            a.AnswerText,
            a.IsCorrect,
            a.OrderIndex
        )).ToList();

        // Calculate statistics
        var totalAnswers = answersList.Count;
        var correctAnswersCount = answersList.Count(a => a.IsCorrect);

        return new QuestionAnswersResponse(
            question.Id,
            question.QuestionText,
            question.QuestionType,
            answerSummaries,
            totalAnswers,
            correctAnswersCount
        );
    }
}