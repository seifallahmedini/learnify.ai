using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record ReorderAnswersCommand(
    int QuestionId,
    IEnumerable<AnswerOrderItem> AnswerOrders
) : ICommand<AnswerReorderResponse>;

public class ReorderAnswersValidator : AbstractValidator<ReorderAnswersCommand>
{
    public ReorderAnswersValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.AnswerOrders)
            .NotEmpty()
            .WithMessage("Answer orders cannot be empty");

        RuleForEach(x => x.AnswerOrders).ChildRules(order =>
        {
            order.RuleFor(o => o.AnswerId)
                .GreaterThan(0)
                .WithMessage("Answer ID must be greater than 0");

            order.RuleFor(o => o.OrderIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Order index must be 0 or greater");
        });
    }
}

public class ReorderAnswersHandler : IRequestHandler<ReorderAnswersCommand, AnswerReorderResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public ReorderAnswersHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerReorderResponse> Handle(ReorderAnswersCommand request, CancellationToken cancellationToken)
    {
        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            throw new ArgumentException($"Question with ID {request.QuestionId} not found");

        // Get all answers for the question
        var answers = await _answerRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
        var answersList = answers.ToList();

        var results = new List<AnswerOrderResult>();
        var success = true;
        var errorMessages = new List<string>();

        foreach (var orderItem in request.AnswerOrders)
        {
            var answer = answersList.FirstOrDefault(a => a.Id == orderItem.AnswerId);
            if (answer == null)
            {
                results.Add(new AnswerOrderResult(
                    orderItem.AnswerId,
                    -1,
                    orderItem.OrderIndex,
                    false
                ));
                errorMessages.Add($"Answer with ID {orderItem.AnswerId} not found");
                success = false;
                continue;
            }

            // Verify answer belongs to the question
            if (answer.QuestionId != request.QuestionId)
            {
                results.Add(new AnswerOrderResult(
                    orderItem.AnswerId,
                    answer.OrderIndex,
                    orderItem.OrderIndex,
                    false
                ));
                errorMessages.Add($"Answer with ID {orderItem.AnswerId} does not belong to question {request.QuestionId}");
                success = false;
                continue;
            }

            var oldOrderIndex = answer.OrderIndex;
            
            try
            {
                // Update order index
                answer.OrderIndex = orderItem.OrderIndex;
                answer.UpdatedAt = DateTime.UtcNow;
                await _answerRepository.UpdateAsync(answer, cancellationToken);

                results.Add(new AnswerOrderResult(
                    orderItem.AnswerId,
                    oldOrderIndex,
                    orderItem.OrderIndex,
                    true
                ));
            }
            catch (Exception ex)
            {
                results.Add(new AnswerOrderResult(
                    orderItem.AnswerId,
                    oldOrderIndex,
                    orderItem.OrderIndex,
                    false
                ));
                errorMessages.Add($"Failed to update answer {orderItem.AnswerId}: {ex.Message}");
                success = false;
            }
        }

        var message = success 
            ? "All answers reordered successfully" 
            : $"Reordering completed with {errorMessages.Count} errors: {string.Join(", ", errorMessages)}";

        return new AnswerReorderResponse(
            request.QuestionId,
            results,
            success,
            message
        );
    }
}
