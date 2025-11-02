using learnify.ai.api.Common.Enums;

namespace learnify.ai.api.Features.Users;

public record UserDashboardResponse(
    int UserId,
    RoleType Role,
    UserDashboardStats Stats,
    IEnumerable<RecentActivityItem> RecentActivity,
    IEnumerable<ActiveEnrollmentSummary> ActiveEnrollments,
    QuizPerformanceSummary QuizPerformance,
    InstructorDashboardData? InstructorData = null
);

public record UserDashboardStats(
    int TotalEnrollments,
    int ActiveEnrollments,
    int CompletedCourses,
    decimal OverallProgress,
    int TotalTimeSpent, // in minutes
    int CertificatesEarned,
    int QuizzesTaken,
    int QuizzesPassed
);

public record RecentActivityItem(
    string ActivityType, // "enrollment", "lesson_completed", "quiz_completed", "course_completed"
    string Title,
    string Description,
    DateTime ActivityDate,
    int? RelatedCourseId = null,
    int? RelatedLessonId = null,
    int? RelatedQuizId = null
);

public record ActiveEnrollmentSummary(
    int EnrollmentId,
    int CourseId,
    string CourseTitle,
    string CourseThumbnail,
    decimal Progress,
    DateTime LastAccessDate,
    int CompletedLessons,
    int TotalLessons,
    string InstructorName,
    DateTime EnrollmentDate
);

public record QuizPerformanceSummary(
    int TotalQuizzesTaken,
    int QuizzesPassed,
    int QuizzesFailed,
    decimal PassRate,
    double AverageScore,
    int BestScore,
    int RecentQuizzesCount, // last 30 days
    IEnumerable<RecentQuizAttempt> RecentAttempts
);

public record RecentQuizAttempt(
    int QuizId,
    string QuizTitle,
    string CourseTitle,
    int Score,
    int MaxScore,
    decimal ScorePercentage,
    bool IsPassed,
    DateTime AttemptDate
);

public record InstructorDashboardData(
    int TotalCourses,
    int PublishedCourses,
    int DraftCourses,
    int TotalStudents,
    decimal TotalRevenue,
    decimal MonthlyRevenue,
    double AverageRating,
    int TotalReviews,
    IEnumerable<TopPerformingCourse> TopCourses,
    CourseAnalyticsSummary RecentAnalytics
);

public record TopPerformingCourse(
    int CourseId,
    string Title,
    int StudentCount,
    decimal Revenue,
    double AverageRating,
    int ReviewCount,
    decimal CompletionRate
);

public record CourseAnalyticsSummary(
    int NewEnrollmentsThisMonth,
    int CompletionsThisMonth,
    decimal RevenueThisMonth,
    decimal RevenueGrowth, // percentage
    int ActiveStudents
);
