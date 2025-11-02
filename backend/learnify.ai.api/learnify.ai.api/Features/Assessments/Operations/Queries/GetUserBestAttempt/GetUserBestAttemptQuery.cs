using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record GetUserBestAttemptQuery(
    int UserId,
    int QuizId
) : IQuery<QuizAttemptDetailResponse?>;

public class GetUserBestAttemptValidator : AbstractValidator<GetUserBestAttemptQuery>
{
    public GetUserBestAttemptValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class GetUserBestAttemptHandler : IRequestHandler<GetUserBestAttemptQuery, QuizAttemptDetailResponse?>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IUserRepository _userRepository;

    public GetUserBestAttemptHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IUserRepository userRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _userRepository = userRepository;
    }

    public async Task<QuizAttemptDetailResponse?> Handle(GetUserBestAttemptQuery request, CancellationToken cancellationToken)
    {
        // Verify user and quiz exist
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        // Get best attempt
        var bestAttempt = await _quizAttemptRepository.GetBestAttemptAsync(request.UserId, request.QuizId, cancellationToken);
        if (bestAttempt == null)
            return null;

        return new QuizAttemptDetailResponse(
            bestAttempt.Id,
            bestAttempt.QuizId,
            quiz.Title,
            bestAttempt.UserId,
            user.GetFullName(),
            bestAttempt.StartedAt,
            bestAttempt.CompletedAt,
            bestAttempt.IsCompleted() ? bestAttempt.Score : null,
            bestAttempt.MaxScore,
            bestAttempt.IsCompleted() ? bestAttempt.GetScorePercentage() : 0,
            bestAttempt.IsCompleted(),
            bestAttempt.IsPassed,
            bestAttempt.TimeSpent,
            bestAttempt.GetFormattedTimeSpent(),
            null // No time remaining for completed attempts
        );
    }
}
