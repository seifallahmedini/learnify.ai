using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record DeleteAnswerCommand(
    int Id
) : ICommand<bool>;

public class DeleteAnswerValidator : AbstractValidator<DeleteAnswerCommand>
{
    public DeleteAnswerValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Answer ID must be greater than 0");
    }
}

public class DeleteAnswerHandler : IRequestHandler<DeleteAnswerCommand, bool>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;

    public DeleteAnswerHandler(
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
    }

    public async Task<bool> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return false;

        // Check business rules before deletion
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId, cancellationToken);
        if (question != null)
        {
            var allAnswers = await _answerRepository.GetByQuestionIdAsync(answer.QuestionId, cancellationToken);
            var answersList = allAnswers.ToList();

            // For True/False questions, must have at least 2 answers
            if (question.IsTrueFalse() && answersList.Count <= 2)
            {
                throw new InvalidOperationException("True/False questions must have at least 2 answers");
            }

            // For Multiple Choice questions, must have at least 2 answers
            if (question.IsMultipleChoice() && answersList.Count <= 2)
            {
                throw new InvalidOperationException("Multiple choice questions must have at least 2 answers");
            }

            // If this is the only correct answer, prevent deletion
            if (answer.IsCorrect)
            {
                var correctAnswers = answersList.Where(a => a.IsCorrect && a.Id != answer.Id).ToList();
                if (!correctAnswers.Any() && (question.IsMultipleChoice() || question.IsTrueFalse()))
                {
                    throw new InvalidOperationException("Cannot delete the only correct answer. Please mark another answer as correct first.");
                }
            }
        }

        return await _answerRepository.DeleteAsync(answer, cancellationToken);
    }
}
