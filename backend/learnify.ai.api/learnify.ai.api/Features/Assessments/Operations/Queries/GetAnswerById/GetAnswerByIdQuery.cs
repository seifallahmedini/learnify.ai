using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record GetAnswerByIdQuery(
    int Id
) : IQuery<AnswerResponse?>;

public class GetAnswerByIdValidator : AbstractValidator<GetAnswerByIdQuery>
{
    public GetAnswerByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Answer ID must be greater than 0");
    }
}

public class GetAnswerByIdHandler : IRequestHandler<GetAnswerByIdQuery, AnswerResponse?>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public GetAnswerByIdHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerResponse?> Handle(GetAnswerByIdQuery request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return null;

        // Load question data
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId, cancellationToken);

        return new AnswerResponse(
            answer.Id,
            answer.QuestionId,
            question?.QuestionText ?? "Unknown Question",
            answer.AnswerText,
            answer.IsCorrect,
            answer.OrderIndex,
            answer.CreatedAt,
            answer.UpdatedAt
        );
    }
}
