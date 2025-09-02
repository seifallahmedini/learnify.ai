using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Assessments;

public record ValidateAnswerQuery(
    int AnswerId
) : IQuery<AnswerValidationResponse>;

public class ValidateAnswerValidator : AbstractValidator<ValidateAnswerQuery>
{
    public ValidateAnswerValidator()
    {
        RuleFor(x => x.AnswerId)
            .GreaterThan(0)
            .WithMessage("Answer ID must be greater than 0");
    }
}

public class ValidateAnswerHandler : IRequestHandler<ValidateAnswerQuery, AnswerValidationResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public ValidateAnswerHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerValidationResponse> Handle(ValidateAnswerQuery request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.AnswerId, cancellationToken);
        if (answer == null)
        {
            return new AnswerValidationResponse(
                request.AnswerId,
                false,
                "Answer not found",
                new[] { "Answer with the specified ID does not exist" }
            );
        }

        var question = await _questionRepository.GetByIdAsync(answer.QuestionId, cancellationToken);
        if (question == null)
        {
            return new AnswerValidationResponse(
                request.AnswerId,
                false,
                "Associated question not found",
                new[] { "The question associated with this answer does not exist" }
            );
        }

        var validationErrors = new List<string>();
        var isValid = true;

        // Validate answer text
        if (string.IsNullOrWhiteSpace(answer.AnswerText))
        {
            validationErrors.Add("Answer text cannot be empty");
            isValid = false;
        }

        if (answer.AnswerText.Length > 1000)
        {
            validationErrors.Add("Answer text cannot exceed 1000 characters");
            isValid = false;
        }

        // Validate business rules based on question type
        var allAnswers = await _answerRepository.GetByQuestionIdAsync(answer.QuestionId, cancellationToken);
        var answersList = allAnswers.ToList();

        if (question.IsTrueFalse())
        {
            if (answersList.Count > 2)
            {
                validationErrors.Add("True/False questions cannot have more than 2 answers");
                isValid = false;
            }

            var correctAnswers = answersList.Where(a => a.IsCorrect).ToList();
            if (correctAnswers.Count > 1)
            {
                validationErrors.Add("True/False questions can only have one correct answer");
                isValid = false;
            }
        }

        if (question.IsMultipleChoice())
        {
            var correctAnswers = answersList.Where(a => a.IsCorrect).ToList();
            if (correctAnswers.Count > 1)
            {
                validationErrors.Add("Multiple choice questions can only have one correct answer");
                isValid = false;
            }

            if (answersList.Count < 2)
            {
                validationErrors.Add("Multiple choice questions must have at least 2 answers");
                isValid = false;
            }
        }

        // Validate order index uniqueness
        var duplicateOrderAnswers = answersList.Where(a => a.OrderIndex == answer.OrderIndex && a.Id != answer.Id).ToList();
        if (duplicateOrderAnswers.Any())
        {
            validationErrors.Add($"Another answer already has order index {answer.OrderIndex}");
            isValid = false;
        }

        var message = isValid ? "Answer is valid" : "Answer has validation errors";

        return new AnswerValidationResponse(
            answer.Id,
            isValid,
            message,
            validationErrors
        );
    }
}