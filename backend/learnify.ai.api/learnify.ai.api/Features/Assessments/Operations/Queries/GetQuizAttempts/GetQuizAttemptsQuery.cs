using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record GetQuizAttemptsQuery(
    int QuizId,
    int Page = 1,
    int PageSize = 10
) : IQuery<QuizAttemptsResponse>;

public class GetQuizAttemptsValidator : AbstractValidator<GetQuizAttemptsQuery>
{
    public GetQuizAttemptsValidator()
    {
        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetQuizAttemptsHandler : IRequestHandler<GetQuizAttemptsQuery, QuizAttemptsResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IUserRepository _userRepository;

    public GetQuizAttemptsHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository,
        IUserRepository userRepository)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _userRepository = userRepository;
    }

    public async Task<QuizAttemptsResponse> Handle(GetQuizAttemptsQuery request, CancellationToken cancellationToken)
    {
        // Verify quiz exists
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        // Get all attempts for the quiz
        var attempts = await _quizAttemptRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);
        var attemptsList = attempts.OrderByDescending(a => a.StartedAt).ToList();

        // Get total count before pagination
        var totalCount = attemptsList.Count;

        // Apply pagination
        var pagedAttempts = attemptsList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build attempt responses
        var attemptResponses = new List<QuizAttemptResponse>();
        foreach (var attempt in pagedAttempts)
        {
            var user = await _userRepository.GetByIdAsync(attempt.UserId, cancellationToken);
            
            int? timeRemainingMinutes = null;
            if (quiz.HasTimeLimit() && !attempt.IsCompleted())
            {
                var elapsed = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
                var remaining = quiz.TimeLimit!.Value - elapsed;
                timeRemainingMinutes = remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
            }

            attemptResponses.Add(new QuizAttemptResponse(
                attempt.Id,
                attempt.QuizId,
                quiz.Title,
                attempt.UserId,
                user?.GetFullName() ?? "Unknown User",
                attempt.StartedAt,
                attempt.CompletedAt,
                attempt.IsCompleted() ? attempt.Score : null,
                attempt.MaxScore,
                attempt.IsCompleted(),
                attempt.TimeSpent,
                timeRemainingMinutes
            ));
        }

        // Calculate statistics
        var completedAttempts = attemptsList.Where(a => a.IsCompleted()).ToList();
        var averageScore = completedAttempts.Any() 
            ? completedAttempts.Average(a => a.GetScorePercentage()) 
            : 0;

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new QuizAttemptsResponse(
            quiz.Id,
            quiz.Title,
            attemptResponses,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            Math.Round(averageScore, 2),
            completedAttempts.Count
        );
    }
}