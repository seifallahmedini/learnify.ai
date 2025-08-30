namespace learnify.ai.api.Features.Enrollments;

// Response DTOs for Enrollment endpoints
public record EnrollmentResponse(
    int Id,
    int UserId,
    string UserName,
    int CourseId,
    string CourseTitle,
    DateTime EnrollmentDate,
    DateTime? CompletionDate,
    decimal Progress,
    EnrollmentStatus Status,
    int? PaymentId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record EnrollmentDetailsResponse(
    int Id,
    int UserId,
    string UserName,
    string UserEmail,
    int CourseId,
    string CourseTitle,
    string CourseDescription,
    DateTime EnrollmentDate,
    DateTime? CompletionDate,
    decimal Progress,
    EnrollmentStatus Status,
    int CompletedLessons,
    int TotalLessons,
    int TotalTimeSpentMinutes,
    int? PaymentId,
    decimal? PaymentAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record EnrollmentProgressResponse(
    int EnrollmentId,
    int UserId,
    int CourseId,
    string CourseTitle,
    decimal OverallProgress,
    int CompletedLessons,
    int TotalLessons,
    int TotalTimeSpentMinutes,
    string FormattedTimeSpent,
    IEnumerable<LessonProgressResponse> LessonProgress,
    DateTime LastAccessDate
);

public record LessonProgressResponse(
    int LessonId,
    string LessonTitle,
    bool IsCompleted,
    DateTime? CompletionDate,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    DateTime LastAccessDate
);

public record CourseEnrollmentsResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<EnrollmentResponse> Enrollments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record EnrollmentStatisticsResponse(
    int TotalEnrollments,
    int ActiveEnrollments,
    int CompletedEnrollments,
    int DroppedEnrollments,
    int SuspendedEnrollments,
    decimal CompletionRate,
    decimal DropoutRate,
    int AverageTimeToCompletion,
    DateTime? StatisticsDate
);

public record CompletionStatusResponse(
    int EnrollmentId,
    bool IsCompleted,
    DateTime? CompletionDate,
    decimal Progress,
    int CompletedLessons,
    int TotalLessons,
    bool EligibleForCertificate,
    string? CertificateUrl
);

public record CertificateResponse(
    int EnrollmentId,
    string CertificateId,
    string CertificateUrl,
    DateTime IssuedDate,
    string StudentName,
    string CourseTitle,
    string InstructorName,
    DateTime CompletionDate,
    decimal FinalScore
);

public record PopularCourseResponse(
    int CourseId,
    string Title,
    string InstructorName,
    int EnrollmentCount,
    decimal AverageRating,
    decimal Price,
    string ThumbnailUrl
);

public record TrendingCategoryResponse(
    int CategoryId,
    string Name,
    int RecentEnrollments,
    decimal GrowthRate,
    string IconUrl
);