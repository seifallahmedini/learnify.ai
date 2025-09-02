using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Assessments;

public record CreateAnswerCommand(
    int QuestionId,
    string AnswerText,
    bool IsCorrect = false,
    int? OrderIndex = null
) : ICommand<AnswerResponse>;

public class CreateAnswerValidator : AbstractValidator<CreateAnswerCommand>
{
    public CreateAnswerValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .WithMessage("Answer text is required")
            .MaximumLength(1000)
            .WithMessage("Answer text cannot exceed 1000 characters");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .When(x => x.OrderIndex.HasValue)
            .WithMessage("Order index must be 0 or greater");
    }
}

public class CreateAnswerHandler : IRequestHandler<CreateAnswerCommand, AnswerResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public CreateAnswerHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerResponse> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            throw new ArgumentException($"Question with ID {request.QuestionId} not found");

        // For True/False questions, ensure we don't have more than 2 answers
        if (question.IsTrueFalse())
        {
            var existingAnswers = await _answerRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
            if (existingAnswers.Count() >= 2)
                throw new InvalidOperationException("True/False questions can only have 2 answers");
        }

        // For Single Choice questions, ensure only one correct answer
        if (question.IsMultipleChoice() && request.IsCorrect)
        {
            var hasCorrectAnswer = await _answerRepository.HasCorrectAnswerAsync(request.QuestionId, cancellationToken);
            if (hasCorrectAnswer)
                throw new InvalidOperationException("This question already has a correct answer. Please update the existing correct answer or set this answer as incorrect.");
        }

        // Determine order index
        var orderIndex = request.OrderIndex ?? await _answerRepository.GetMaxOrderIndexAsync(request.QuestionId, cancellationToken) + 1;

        // Create answer
        var answer = new Answer
        {
            QuestionId = request.QuestionId,
            AnswerText = request.AnswerText,
            IsCorrect = request.IsCorrect,
            OrderIndex = orderIndex,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdAnswer = await _answerRepository.CreateAsync(answer, cancellationToken);

        return new AnswerResponse(
            createdAnswer.Id,
            createdAnswer.QuestionId,
            question.QuestionText,
            createdAnswer.AnswerText,
            createdAnswer.IsCorrect,
            createdAnswer.OrderIndex,
            createdAnswer.CreatedAt,
            createdAnswer.UpdatedAt
        );
    }
}