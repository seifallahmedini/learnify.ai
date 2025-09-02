using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record GetUserQuizAttemptsQuery(
    int UserId,
    int? QuizId = null,
    bool? IsCompleted = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<UserQuizAttemptsResponse>;

public class GetUserQuizAttemptsValidator : AbstractValidator<GetUserQuizAttemptsQuery>
{
    public GetUserQuizAttemptsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .When(x => x.QuizId.HasValue)
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

public class GetUserQuizAttemptsHandler : IRequestHandler<GetUserQuizAttemptsQuery, UserQuizAttemptsResponse>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;

    public GetUserQuizAttemptsHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IUserRepository userRepository,
        IQuizRepository quizRepository,
        ICourseRepository courseRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _userRepository = userRepository;
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
    }

    public async Task<UserQuizAttemptsResponse> Handle(GetUserQuizAttemptsQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Get user attempts
        var attempts = await _quizAttemptRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var attemptsList = attempts.ToList();

        // Apply filters
        if (request.QuizId.HasValue)
        {
            attemptsList = attemptsList.Where(a => a.QuizId == request.QuizId.Value).ToList();
        }

        if (request.IsCompleted.HasValue)
        {
            attemptsList = attemptsList.Where(a => a.IsCompleted() == request.IsCompleted.Value).ToList();
        }

        // Get total count before pagination
        var totalCount = attemptsList.Count;

        // Apply pagination
        var pagedAttempts = attemptsList
            .OrderByDescending(a => a.StartedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build attempt summaries
        var attemptSummaries = new List<UserQuizAttemptSummaryResponse>();
        foreach (var attempt in pagedAttempts)
        {
            var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
            var course = quiz != null ? await _courseRepository.GetByIdAsync(quiz.CourseId, cancellationToken) : null;

            attemptSummaries.Add(new UserQuizAttemptSummaryResponse(
                attempt.Id,
                attempt.QuizId,
                quiz?.Title ?? "Unknown Quiz",
                quiz?.CourseId ?? 0,
                course?.Title ?? "Unknown Course",
                attempt.StartedAt,
                attempt.CompletedAt,
                attempt.IsCompleted() ? attempt.Score : null,
                attempt.MaxScore,
                attempt.IsCompleted() ? attempt.GetScorePercentage() : null,
                attempt.IsCompleted(),
                attempt.IsPassed,
                attempt.TimeSpent,
                attempt.GetFormattedTimeSpent()
            ));
        }

        // Calculate statistics
        var completedAttempts = attemptsList.Where(a => a.IsCompleted()).ToList();
        var passedAttempts = completedAttempts.Where(a => a.IsPassed).ToList();
        var failedAttempts = completedAttempts.Where(a => !a.IsPassed).ToList();

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

        var stats = new QuizAttemptStatsResponse(
            attemptsList.Count,
            completedAttempts.Count,
            passedAttempts.Count,
            failedAttempts.Count,
            Math.Round(averageScore, 2),
            bestScore,
            totalTimeSpent,
            formattedTotalTime
        );

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new UserQuizAttemptsResponse(
            user.Id,
            user.GetFullName(),
            attemptSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            stats
        );
    }
}