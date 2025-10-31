using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Assessments;

public record CreateMultipleAnswersCommand(
    int QuestionId,
    IEnumerable<CreateSingleAnswerRequest> Answers
) : ICommand<BulkAnswerOperationResponse>;

public class CreateMultipleAnswersValidator : AbstractValidator<CreateMultipleAnswersCommand>
{
    public CreateMultipleAnswersValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.Answers)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one answer must be provided");

        RuleForEach(x => x.Answers)
            .SetValidator(new CreateSingleAnswerValidator());
    }
}

public class CreateSingleAnswerValidator : AbstractValidator<CreateSingleAnswerRequest>
{
    public CreateSingleAnswerValidator()
    {
        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .WithMessage("Answer text is required")
            .MaximumLength(1000)
            .WithMessage("Answer text cannot exceed 1000 characters");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order index must be 0 or greater");
    }
}

public class CreateMultipleAnswersHandler : IRequestHandler<CreateMultipleAnswersCommand, BulkAnswerOperationResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public CreateMultipleAnswersHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<BulkAnswerOperationResponse> Handle(CreateMultipleAnswersCommand request, CancellationToken cancellationToken)
    {
        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
        {
            return new BulkAnswerOperationResponse(
                Success: false,
                ProcessedCount: 0,
                SuccessCount: 0,
                FailureCount: 0,
                Errors: new[] { $"Question with ID {request.QuestionId} not found" },
                Message: "Question not found"
            );
        }

        var createdAnswers = new List<AnswerResponse>();
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        // Get existing answers for validation
        var existingAnswers = await _answerRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
        var correctAnswersCount = existingAnswers.Count(a => a.IsCorrect);
        
        // Get max order index for auto-ordering
        var maxOrderIndex = existingAnswers.Any() 
            ? existingAnswers.Max(a => a.OrderIndex) 
            : 0;

        foreach (var answerRequest in request.Answers)
        {
            try
            {
                // Validate business rules for each answer
                if (question.IsTrueFalse() && existingAnswers.Count() + successCount >= 2)
                {
                    errors.Add($"True/False questions can only have 2 answers. Answer '{answerRequest.AnswerText}' was skipped.");
                    failureCount++;
                    continue;
                }

                if (question.IsMultipleChoice() && answerRequest.IsCorrect && (correctAnswersCount + (createdAnswers.Count(a => a.IsCorrect)) > 0))
                {
                    errors.Add($"This question already has a correct answer. Answer '{answerRequest.AnswerText}' was set as incorrect.");
                    // Continue but mark as incorrect
                }

                // Determine order index
                var orderIndex = answerRequest.OrderIndex > 0 ? answerRequest.OrderIndex : ++maxOrderIndex;

                // Create answer
                var answer = new Answer
                {
                    QuestionId = request.QuestionId,
                    AnswerText = answerRequest.AnswerText,
                    IsCorrect = question.IsMultipleChoice() && answerRequest.IsCorrect && (correctAnswersCount + createdAnswers.Count(a => a.IsCorrect) > 0) 
                        ? false // Force to false if already have correct answer
                        : answerRequest.IsCorrect,
                    OrderIndex = orderIndex,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdAnswer = await _answerRepository.CreateAsync(answer, cancellationToken);

                var answerResponse = new AnswerResponse(
                    createdAnswer.Id,
                    createdAnswer.QuestionId,
                    question.QuestionText,
                    createdAnswer.AnswerText,
                    createdAnswer.IsCorrect,
                    createdAnswer.OrderIndex,
                    createdAnswer.CreatedAt,
                    createdAnswer.UpdatedAt
                );

                createdAnswers.Add(answerResponse);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to create answer '{answerRequest.AnswerText}': {ex.Message}");
                failureCount++;
            }
        }

        var isSuccess = successCount > 0;
        var message = isSuccess 
            ? $"Successfully created {successCount} answer(s)" + (failureCount > 0 ? $" with {failureCount} failure(s)" : "")
            : "Failed to create any answers";

        return new BulkAnswerOperationResponse(
            Success: isSuccess,
            ProcessedCount: request.Answers.Count(),
            SuccessCount: successCount,
            FailureCount: failureCount,
            Errors: errors,
            Message: message,
            CreatedAnswers: createdAnswers
        );
    }
}