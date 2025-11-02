using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record UpdateAnswerCommand(
    int Id,
    string? AnswerText = null,
    bool? IsCorrect = null,
    int? OrderIndex = null
) : ICommand<AnswerResponse?>;

public class UpdateAnswerValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Answer ID must be greater than 0");

        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.AnswerText))
            .WithMessage("Answer text cannot exceed 1000 characters and cannot be empty");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .When(x => x.OrderIndex.HasValue)
            .WithMessage("Order index must be 0 or greater");
    }
}

public class UpdateAnswerHandler : IRequestHandler<UpdateAnswerCommand, AnswerResponse?>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public UpdateAnswerHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerResponse?> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return null;

        var question = await _questionRepository.GetByIdAsync(answer.QuestionId, cancellationToken);
        if (question == null)
            throw new InvalidOperationException("Associated question not found");

        // Validate correct answer constraints
        if (request.IsCorrect.HasValue && request.IsCorrect.Value && !answer.IsCorrect)
        {
            // For Single Choice questions, ensure only one correct answer
            if (question.IsMultipleChoice())
            {
                var hasCorrectAnswer = await _answerRepository.HasCorrectAnswerAsync(answer.QuestionId, cancellationToken);
                if (hasCorrectAnswer)
                    throw new InvalidOperationException("This question already has a correct answer. Please update the existing correct answer first.");
            }
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.AnswerText))
        {
            answer.AnswerText = request.AnswerText;
        }

        if (request.IsCorrect.HasValue)
        {
            answer.IsCorrect = request.IsCorrect.Value;
        }

        if (request.OrderIndex.HasValue)
        {
            // Check if another answer already has this order index
            var existingAnswer = await _answerRepository.GetByOrderIndexAsync(answer.QuestionId, request.OrderIndex.Value, cancellationToken);
            if (existingAnswer != null && existingAnswer.Id != answer.Id)
            {
                // Swap order indices
                existingAnswer.OrderIndex = answer.OrderIndex;
                existingAnswer.UpdatedAt = DateTime.UtcNow;
                await _answerRepository.UpdateAsync(existingAnswer, cancellationToken);
            }
            
            answer.OrderIndex = request.OrderIndex.Value;
        }

        answer.UpdatedAt = DateTime.UtcNow;
        var updatedAnswer = await _answerRepository.UpdateAsync(answer, cancellationToken);

        return new AnswerResponse(
            updatedAnswer.Id,
            updatedAnswer.QuestionId,
            question.QuestionText,
            updatedAnswer.AnswerText,
            updatedAnswer.IsCorrect,
            updatedAnswer.OrderIndex,
            updatedAnswer.CreatedAt,
            updatedAnswer.UpdatedAt
        );
    }
}
