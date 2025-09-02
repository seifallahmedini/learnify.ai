using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record GetUserLatestAttemptQuery(
    int UserId,
    int QuizId
) : IQuery<QuizAttemptDetailResponse?>;

public class GetUserLatestAttemptValidator : AbstractValidator<GetUserLatestAttemptQuery>
{
    public GetUserLatestAttemptValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class GetUserLatestAttemptHandler : IRequestHandler<GetUserLatestAttemptQuery, QuizAttemptDetailResponse?>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IUserRepository _userRepository;

    public GetUserLatestAttemptHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IUserRepository userRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _userRepository = userRepository;
    }

    public async Task<QuizAttemptDetailResponse?> Handle(GetUserLatestAttemptQuery request, CancellationToken cancellationToken)
    {
        // Verify user and quiz exist
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        // Get latest attempt
        var latestAttempt = await _quizAttemptRepository.GetLatestAttemptAsync(request.UserId, request.QuizId, cancellationToken);
        if (latestAttempt == null)
            return null;

        // Calculate time remaining if quiz is in progress and has time limit
        int? timeRemainingMinutes = null;
        if (!latestAttempt.IsCompleted() && quiz.HasTimeLimit())
        {
            var elapsed = (DateTime.UtcNow - latestAttempt.StartedAt).TotalMinutes;
            var remaining = quiz.TimeLimit!.Value - elapsed;
            timeRemainingMinutes = remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
        }

        return new QuizAttemptDetailResponse(
            latestAttempt.Id,
            latestAttempt.QuizId,
            quiz.Title,
            latestAttempt.UserId,
            user.GetFullName(),
            latestAttempt.StartedAt,
            latestAttempt.CompletedAt,
            latestAttempt.IsCompleted() ? latestAttempt.Score : null,
            latestAttempt.MaxScore,
            latestAttempt.IsCompleted() ? latestAttempt.GetScorePercentage() : 0,
            latestAttempt.IsCompleted(),
            latestAttempt.IsPassed,
            latestAttempt.TimeSpent,
            latestAttempt.GetFormattedTimeSpent(),
            timeRemainingMinutes
        );
    }
}