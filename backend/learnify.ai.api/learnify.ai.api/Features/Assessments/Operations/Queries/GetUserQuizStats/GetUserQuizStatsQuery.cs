using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record GetUserQuizStatsQuery(
    int UserId
) : IQuery<QuizAttemptStatsResponse>;

public class GetUserQuizStatsValidator : AbstractValidator<GetUserQuizStatsQuery>
{
    public GetUserQuizStatsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class GetUserQuizStatsHandler : IRequestHandler<GetUserQuizStatsQuery, QuizAttemptStatsResponse>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IUserRepository _userRepository;

    public GetUserQuizStatsHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IUserRepository userRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _userRepository = userRepository;
    }

    public async Task<QuizAttemptStatsResponse> Handle(GetUserQuizStatsQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Get all user attempts
        var attempts = await _quizAttemptRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var attemptsList = attempts.ToList();

        // Calculate statistics
        var completedAttempts = attemptsList.Where(a => a.IsCompleted()).ToList();
        var passedAttempts = await _quizAttemptRepository.GetPassedAttemptsAsync(request.UserId, cancellationToken);
        var failedAttempts = await _quizAttemptRepository.GetFailedAttemptsAsync(request.UserId, cancellationToken);

        var passedCount = passedAttempts.Count();
        var failedCount = failedAttempts.Count();

        var averageScore = completedAttempts.Any() 
            ? completedAttempts.Average(a => a.GetScorePercentage()) 
            : 0;

        var bestScore = completedAttempts.Any() 
            ? completedAttempts.Max(a => a.GetScorePercentage()) 
            : (int?)null;

        var totalTimeSpent = attemptsList.Sum(a => a.TimeSpent);
        var hours = totalTimeSpent / 60;
        var minutes = totalTimeSpent % 60;
        var formattedTotalTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        return new QuizAttemptStatsResponse(
            attemptsList.Count,
            completedAttempts.Count,
            passedCount,
            failedCount,
            Math.Round(averageScore, 2),
            bestScore,
            totalTimeSpent,
            formattedTotalTime
        );
    }
}
