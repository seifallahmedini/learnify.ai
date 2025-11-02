using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Progress;

// Response DTOs for Progress endpoints
public record DetailedLessonProgressResponse(
    int LessonId,
    string LessonTitle,
    int? EnrollmentId,
    bool IsCompleted,
    DateTime? CompletionDate,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    DateTime LastAccessDate
);

public record EnrollmentProgressDetailResponse(
    int EnrollmentId,
    int UserId,
    int CourseId,
    string CourseTitle,
    IEnumerable<DetailedLessonProgressResponse> LessonProgress,
    decimal OverallProgress,
    int CompletedLessons,
    int TotalLessons,
    int TotalTimeSpentMinutes,
    string FormattedTotalTimeSpent,
    DateTime LastAccessDate
);

public record UserLearningStatsResponse(
    int UserId,
    string UserName,
    int TotalTimeSpentMinutes,
    string FormattedTotalTimeSpent,
    int CompletedLessons,
    int CompletedCourses,
    int ActiveEnrollments,
    decimal AverageProgress,
    int TotalEnrollments,
    DateTime? LastLearningActivity,
    IEnumerable<CourseLearningStatsResponse> CourseStats
);

public record CourseLearningStatsResponse(
    int CourseId,
    string CourseTitle,
    decimal Progress,
    int CompletedLessons,
    int TotalLessons,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    EnrollmentStatus Status,
    DateTime LastAccessDate
);

public record CourseProgressStatsResponse(
    int CourseId,
    string CourseTitle,
    decimal AverageProgress,
    decimal CompletionRate,
    int TotalEnrollments,
    int CompletedEnrollments,
    int ActiveEnrollments,
    int AverageTimeToCompleteMinutes,
    string FormattedAverageTimeToComplete,
    DateTime? CalculatedAt
);

public record ProgressDashboardResponse(
    int UserId,
    string UserName,
    IEnumerable<RecentProgressResponse> RecentProgress,
    UserLearningOverviewResponse OverallStats,
    IEnumerable<ActiveCourseProgressResponse> ActiveCourses,
    IEnumerable<AchievementResponse> RecentAchievements
);

public record RecentProgressResponse(
    int LessonId,
    string LessonTitle,
    int CourseId,
    string CourseTitle,
    bool IsCompleted,
    int TimeSpentMinutes,
    DateTime ActivityDate
);

public record UserLearningOverviewResponse(
    int TotalTimeSpentMinutes,
    string FormattedTotalTimeSpent,
    int CompletedLessons,
    int CompletedCourses,
    int ActiveCourses,
    decimal AverageProgress,
    int CurrentStreak,
    DateTime? LastLearningDate
);

public record ActiveCourseProgressResponse(
    int CourseId,
    string CourseTitle,
    string InstructorName,
    decimal Progress,
    int CompletedLessons,
    int TotalLessons,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    DateTime LastAccessDate,
    string? ThumbnailUrl
);

public record AchievementResponse(
    string Title,
    string Description,
    string Type,
    DateTime EarnedDate,
    string? BadgeUrl
);
