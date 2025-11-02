using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Assessments;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Users;

public record GetUserQuizAttemptsQuery(
    int UserId,
    int? CourseId = null,
    bool? IsPassed = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<GetUserQuizAttemptsResponse>;

public record GetUserQuizAttemptsResponse(
    IEnumerable<UserQuizAttemptDto> QuizAttempts,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record UserQuizAttemptDto(
    int Id,
    int QuizId,
    string QuizTitle,
    int CourseId,
    string CourseTitle,
    int Score,
    int MaxScore,
    int ScorePercentage,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int TimeSpent,
    string FormattedTimeSpent,
    bool IsPassed,
    bool IsCompleted
);

public class GetUserQuizAttemptsValidator : AbstractValidator<GetUserQuizAttemptsQuery>
{
    public GetUserQuizAttemptsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .When(x => x.CourseId.HasValue)
            .WithMessage("Course ID must be greater than 0 when specified");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetUserQuizAttemptsHandler : IRequestHandler<GetUserQuizAttemptsQuery, GetUserQuizAttemptsResponse>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;

    public GetUserQuizAttemptsHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        ICourseRepository courseRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
    }

    public async Task<GetUserQuizAttemptsResponse> Handle(GetUserQuizAttemptsQuery request, CancellationToken cancellationToken)
    {
        var allAttempts = await _quizAttemptRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        // Apply filters
        if (request.IsPassed.HasValue)
        {
            allAttempts = allAttempts.Where(qa => qa.IsPassed == request.IsPassed.Value);
        }

        if (request.CourseId.HasValue)
        {
            // Filter by course - need to check quiz course relationship
            // Get all quizzes (both active and inactive) to match attempts
            var courseQuizzes = await _quizRepository.GetByCourseIdAsync(request.CourseId.Value, isActive: null, cancellationToken);
            var courseQuizIds = courseQuizzes.Select(q => q.Id).ToHashSet();
            allAttempts = allAttempts.Where(qa => courseQuizIds.Contains(qa.QuizId));
        }

        var totalCount = allAttempts.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedAttempts = allAttempts
            .OrderByDescending(qa => qa.StartedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var attemptDtos = new List<UserQuizAttemptDto>();

        foreach (var attempt in paginatedAttempts)
        {
            var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(quiz!.CourseId, cancellationToken);

            attemptDtos.Add(new UserQuizAttemptDto(
                attempt.Id,
                quiz.Id,
                quiz.Title,
                course!.Id,
                course.Title,
                attempt.Score,
                attempt.MaxScore,
                attempt.GetScorePercentage(),
                attempt.StartedAt,
                attempt.CompletedAt,
                attempt.TimeSpent,
                attempt.GetFormattedTimeSpent(),
                attempt.IsPassed,
                attempt.IsCompleted()
            ));
        }

        return new GetUserQuizAttemptsResponse(
            attemptDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
