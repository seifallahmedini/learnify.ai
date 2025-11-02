using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Assessments;

public record GetAnswersQuery(
    int? QuestionId = null,
    bool? IsCorrect = null
) : IQuery<AnswerListResponse>;

public class GetAnswersValidator : AbstractValidator<GetAnswersQuery>
{
    public GetAnswersValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .When(x => x.QuestionId.HasValue)
            .WithMessage("Question ID must be greater than 0");
    }
}

public class GetAnswersHandler : IRequestHandler<GetAnswersQuery, AnswerListResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public GetAnswersHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerListResponse> Handle(GetAnswersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Answer> answers;
        string questionText = "All Questions";
        int questionId = 0;

        if (request.QuestionId.HasValue)
        {
            // Get answers for specific question
            var question = await _questionRepository.GetByIdAsync(request.QuestionId.Value, cancellationToken);
            if (question == null)
                throw new ArgumentException($"Question with ID {request.QuestionId} not found");

            answers = await _answerRepository.GetByQuestionIdAsync(request.QuestionId.Value, cancellationToken);
            questionText = question.QuestionText;
            questionId = question.Id;
        }
        else
        {
            // Get all answers
            answers = await _answerRepository.GetAllAsync(cancellationToken);
        }

        var answersList = answers.ToList();

        // Apply filters
        if (request.IsCorrect.HasValue)
        {
            answersList = answersList.Where(a => a.IsCorrect == request.IsCorrect.Value).ToList();
        }

        // Build response
        var answerSummaries = answersList
            .OrderBy(a => a.QuestionId)
            .ThenBy(a => a.OrderIndex)
            .Select(a => new AnswerSummaryResponse(
                a.Id,
                a.QuestionId,
                a.AnswerText,
                a.IsCorrect,
                a.OrderIndex
            ))
            .ToList();

        return new AnswerListResponse(
            answerSummaries,
            answersList.Count,
            questionId,
            questionText
        );
    }
}
