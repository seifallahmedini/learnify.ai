using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record GetProgressDashboardQuery(
    int UserId,
    int? Limit = 10
) : IQuery<ProgressDashboardResponse?>;

public class GetProgressDashboardValidator : AbstractValidator<GetProgressDashboardQuery>
{
    public GetProgressDashboardValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .When(x => x.Limit.HasValue)
            .WithMessage("Limit must be between 1 and 50");
    }
}

public class GetProgressDashboardHandler : IRequestHandler<GetProgressDashboardQuery, ProgressDashboardResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetProgressDashboardHandler(
        IUserRepository userRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<ProgressDashboardResponse?> Handle(GetProgressDashboardQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        var limit = request.Limit ?? 10;

        // Get recent progress activities
        var recentProgress = await GetRecentProgress(request.UserId, limit, cancellationToken);

        // Get overall learning statistics
        var overallStats = await GetUserLearningOverview(request.UserId, cancellationToken);

        // Get active courses with progress
        var activeCourses = await GetActiveCourseProgress(request.UserId, limit, cancellationToken);

        // Get recent achievements (placeholder for now)
        var achievements = await GetRecentAchievements(request.UserId, limit, cancellationToken);

        return new ProgressDashboardResponse(
            user.Id,
            user.GetFullName(),
            recentProgress,
            overallStats,
            activeCourses,
            achievements
        );
    }

    private async Task<IEnumerable<RecentProgressResponse>> GetRecentProgress(int userId, int limit, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var recentProgress = new List<RecentProgressResponse>();

        foreach (var enrollment in enrollments)
        {
            var progressList = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);

            foreach (var progress in progressList.OrderByDescending(p => p.LastAccessDate).Take(limit))
            {
                var lesson = await _lessonRepository.GetByIdAsync(progress.LessonId, cancellationToken);
                recentProgress.Add(new RecentProgressResponse(
                    progress.LessonId,
                    lesson?.Title ?? "Unknown Lesson",
                    enrollment.CourseId,
                    course?.Title ?? "Unknown Course",
                    progress.IsCompleted,
                    progress.TimeSpent,
                    progress.LastAccessDate
                ));
            }
        }

        return recentProgress
            .OrderByDescending(p => p.ActivityDate)
            .Take(limit)
            .ToList();
    }

    private async Task<UserLearningOverviewResponse> GetUserLearningOverview(int userId, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var enrollmentsList = enrollments.ToList();

        var totalTimeSpent = 0;
        var totalCompletedLessons = 0;
        DateTime? lastLearningDate = null;

        foreach (var enrollment in enrollmentsList)
        {
            var timeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollment.Id, cancellationToken);
            var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
            
            totalTimeSpent += timeSpent;
            totalCompletedLessons += completedLessons;

            var progressList = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
            var enrollmentLastAccess = progressList.Any() 
                ? progressList.Max(p => p.LastAccessDate) 
                : enrollment.UpdatedAt;

            if (lastLearningDate == null || enrollmentLastAccess > lastLearningDate)
            {
                lastLearningDate = enrollmentLastAccess;
            }
        }

        var completedCourses = enrollmentsList.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Completed);
        var activeCourses = enrollmentsList.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active);
        var averageProgress = enrollmentsList.Any() ? enrollmentsList.Average(e => e.Progress) : 0;

        // Calculate current streak (placeholder logic)
        var currentStreak = CalculateCurrentStreak(enrollmentsList);

        var hours = totalTimeSpent / 60;
        var minutes = totalTimeSpent % 60;
        var formattedTotalTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        return new UserLearningOverviewResponse(
            totalTimeSpent,
            formattedTotalTime,
            totalCompletedLessons,
            completedCourses,
            activeCourses,
            Math.Round(averageProgress, 2),
            currentStreak,
            lastLearningDate
        );
    }

    private async Task<IEnumerable<ActiveCourseProgressResponse>> GetActiveCourseProgress(int userId, int limit, CancellationToken cancellationToken)
    {
        var activeEnrollments = await _enrollmentRepository.GetActiveEnrollmentsAsync(userId, cancellationToken);
        var activeCourseProgress = new List<ActiveCourseProgressResponse>();

        foreach (var enrollment in activeEnrollments.Take(limit))
        {
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
            var instructor = course != null ? await _userRepository.GetByIdAsync(course.InstructorId, cancellationToken) : null;
            var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
            var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);
            var timeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollment.Id, cancellationToken);
            
            var progressList = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
            var lastAccess = progressList.Any() 
                ? progressList.Max(p => p.LastAccessDate) 
                : enrollment.UpdatedAt;

            var hours = timeSpent / 60;
            var minutes = timeSpent % 60;
            var formattedTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

            activeCourseProgress.Add(new ActiveCourseProgressResponse(
                enrollment.CourseId,
                course?.Title ?? "Unknown Course",
                instructor?.GetFullName() ?? "Unknown Instructor",
                enrollment.Progress,
                completedLessons,
                totalLessons,
                timeSpent,
                formattedTime,
                lastAccess,
                course?.ThumbnailUrl
            ));
        }

        return activeCourseProgress.OrderByDescending(c => c.LastAccessDate).ToList();
    }

    private async Task<IEnumerable<AchievementResponse>> GetRecentAchievements(int userId, int limit, CancellationToken cancellationToken)
    {
        // Placeholder implementation for achievements
        // In a real application, you would have an achievements system
        var achievements = new List<AchievementResponse>();

        var completedEnrollments = await _enrollmentRepository.GetCompletedEnrollmentsAsync(userId, cancellationToken);
        var completedCourses = completedEnrollments.Take(limit);

        foreach (var enrollment in completedCourses)
        {
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
            if (enrollment.CompletionDate.HasValue)
            {
                achievements.Add(new AchievementResponse(
                    "Course Completed",
                    $"Successfully completed {course?.Title ?? "Unknown Course"}",
                    "Course Completion",
                    enrollment.CompletionDate.Value,
                    null
                ));
            }
        }

        return achievements.OrderByDescending(a => a.EarnedDate).ToList();
    }

    private int CalculateCurrentStreak(List<Enrollment> enrollments)
    {
        // Placeholder implementation for calculating learning streak
        // In a real application, you would track daily learning activities
        return enrollments.Count(e => e.UpdatedAt >= DateTime.UtcNow.AddDays(-7));
    }
}
