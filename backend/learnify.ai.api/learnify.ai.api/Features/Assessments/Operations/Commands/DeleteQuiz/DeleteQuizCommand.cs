using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Assessments;

public record DeleteQuizCommand(
    int Id
) : ICommand<bool>;

public class DeleteQuizValidator : AbstractValidator<DeleteQuizCommand>
{
    public DeleteQuizValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class DeleteQuizHandler : IRequestHandler<DeleteQuizCommand, bool>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public DeleteQuizHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<bool> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
            return false;

        // Check if quiz has any attempts
        var attempts = await _quizAttemptRepository.GetByQuizIdAsync(request.Id, cancellationToken);
        if (attempts.Any())
        {
            throw new InvalidOperationException("Cannot delete quiz that has student attempts. Consider deactivating it instead.");
        }

        return await _quizRepository.DeleteAsync(quiz, cancellationToken);
    }
}
