using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record BulkAnswerOperationCommand(
    IEnumerable<int> AnswerIds,
    BulkAnswerOperation Operation
) : ICommand<BulkAnswerOperationResponse>;

public class BulkAnswerOperationValidator : AbstractValidator<BulkAnswerOperationCommand>
{
    public BulkAnswerOperationValidator()
    {
        RuleFor(x => x.AnswerIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one answer ID must be provided");

        RuleForEach(x => x.AnswerIds)
            .GreaterThan(0)
            .WithMessage("Answer IDs must be greater than 0");

        RuleFor(x => x.Operation)
            .IsInEnum()
            .WithMessage("Invalid bulk operation type");
    }
}

public class BulkAnswerOperationHandler : IRequestHandler<BulkAnswerOperationCommand, BulkAnswerOperationResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public BulkAnswerOperationHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<BulkAnswerOperationResponse> Handle(BulkAnswerOperationCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        try
        {
            switch (request.Operation)
            {
                case BulkAnswerOperation.Delete:
                    return await HandleBulkDelete(request.AnswerIds, cancellationToken);
                
                case BulkAnswerOperation.MarkCorrect:
                    return await HandleBulkMarkCorrect(request.AnswerIds, cancellationToken);
                
                case BulkAnswerOperation.MarkIncorrect:
                    return await HandleBulkMarkIncorrect(request.AnswerIds, cancellationToken);
                
                case BulkAnswerOperation.Reorder:
                    return await HandleBulkReorder(request.AnswerIds, cancellationToken);
                
                default:
                    return new BulkAnswerOperationResponse(
                        Success: false,
                        ProcessedCount: 0,
                        SuccessCount: 0,
                        FailureCount: 0,
                        Errors: new[] { "Unsupported bulk operation" },
                        Message: "Unsupported operation"
                    );
            }
        }
        catch (Exception ex)
        {
            return new BulkAnswerOperationResponse(
                Success: false,
                ProcessedCount: request.AnswerIds.Count(),
                SuccessCount: 0,
                FailureCount: request.AnswerIds.Count(),
                Errors: new[] { ex.Message },
                Message: "Bulk operation failed"
            );
        }
    }

    private async Task<BulkAnswerOperationResponse> HandleBulkDelete(IEnumerable<int> answerIds, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        foreach (var answerId in answerIds)
        {
            try
            {
                var deleted = await _answerRepository.DeleteAsync(answerId, cancellationToken);
                if (deleted)
                    successCount++;
                else
                {
                    errors.Add($"Answer {answerId} not found");
                    failureCount++;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to delete answer {answerId}: {ex.Message}");
                failureCount++;
            }
        }

        return new BulkAnswerOperationResponse(
            Success: successCount > 0,
            ProcessedCount: answerIds.Count(),
            SuccessCount: successCount,
            FailureCount: failureCount,
            Errors: errors,
            Message: $"Deleted {successCount} answer(s)" + (failureCount > 0 ? $" with {failureCount} failure(s)" : "")
        );
    }

    private async Task<BulkAnswerOperationResponse> HandleBulkMarkCorrect(IEnumerable<int> answerIds, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        foreach (var answerId in answerIds)
        {
            try
            {
                var answer = await _answerRepository.GetByIdAsync(answerId, cancellationToken);
                if (answer == null)
                {
                    errors.Add($"Answer {answerId} not found");
                    failureCount++;
                    continue;
                }

                // Check if question allows multiple correct answers (only Multiple Choice with single answer restriction)
                var question = await _questionRepository.GetByIdAsync(answer.QuestionId, cancellationToken);
                if (question != null && question.IsMultipleChoice())
                {
                    var hasCorrectAnswer = await _answerRepository.HasCorrectAnswerAsync(answer.QuestionId, cancellationToken);
                    if (hasCorrectAnswer && !answer.IsCorrect)
                    {
                        errors.Add($"Question {answer.QuestionId} already has a correct answer. Answer {answerId} was skipped.");
                        failureCount++;
                        continue;
                    }
                }

                answer.IsCorrect = true;
                answer.UpdatedAt = DateTime.UtcNow;
                await _answerRepository.UpdateAsync(answer, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to mark answer {answerId} as correct: {ex.Message}");
                failureCount++;
            }
        }

        return new BulkAnswerOperationResponse(
            Success: successCount > 0,
            ProcessedCount: answerIds.Count(),
            SuccessCount: successCount,
            FailureCount: failureCount,
            Errors: errors,
            Message: $"Marked {successCount} answer(s) as correct" + (failureCount > 0 ? $" with {failureCount} failure(s)" : "")
        );
    }

    private async Task<BulkAnswerOperationResponse> HandleBulkMarkIncorrect(IEnumerable<int> answerIds, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        foreach (var answerId in answerIds)
        {
            try
            {
                var answer = await _answerRepository.GetByIdAsync(answerId, cancellationToken);
                if (answer == null)
                {
                    errors.Add($"Answer {answerId} not found");
                    failureCount++;
                    continue;
                }

                answer.IsCorrect = false;
                answer.UpdatedAt = DateTime.UtcNow;
                await _answerRepository.UpdateAsync(answer, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to mark answer {answerId} as incorrect: {ex.Message}");
                failureCount++;
            }
        }

        return new BulkAnswerOperationResponse(
            Success: successCount > 0,
            ProcessedCount: answerIds.Count(),
            SuccessCount: successCount,
            FailureCount: failureCount,
            Errors: errors,
            Message: $"Marked {successCount} answer(s) as incorrect" + (failureCount > 0 ? $" with {failureCount} failure(s)" : "")
        );
    }

    private async Task<BulkAnswerOperationResponse> HandleBulkReorder(IEnumerable<int> answerIds, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var successCount = 0;
        var failureCount = 0;

        var answerIdsList = answerIds.ToList();
        
        for (int i = 0; i < answerIdsList.Count; i++)
        {
            var answerId = answerIdsList[i];
            try
            {
                var answer = await _answerRepository.GetByIdAsync(answerId, cancellationToken);
                if (answer == null)
                {
                    errors.Add($"Answer {answerId} not found");
                    failureCount++;
                    continue;
                }

                answer.OrderIndex = i + 1; // 1-based ordering
                answer.UpdatedAt = DateTime.UtcNow;
                await _answerRepository.UpdateAsync(answer, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to reorder answer {answerId}: {ex.Message}");
                failureCount++;
            }
        }

        return new BulkAnswerOperationResponse(
            Success: successCount > 0,
            ProcessedCount: answerIds.Count(),
            SuccessCount: successCount,
            FailureCount: failureCount,
            Errors: errors,
            Message: $"Reordered {successCount} answer(s)" + (failureCount > 0 ? $" with {failureCount} failure(s)" : "")
        );
    }
}
