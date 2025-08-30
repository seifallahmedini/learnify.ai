namespace learnify.ai.api.Features.Enrollments;

// Request DTOs for Enrollment endpoints
public record EnrollInCourseRequest(
    int UserId,
    int CourseId,
    int? PaymentId = null
);

public record UpdateEnrollmentStatusRequest(
    EnrollmentStatus Status
);

public record UpdateEnrollmentProgressRequest(
    int LessonId,
    bool IsCompleted = false,
    int TimeSpentMinutes = 0
);

public record GetEnrollmentStatisticsRequest(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? UserId = null,
    int? CourseId = null
);

public record GetCourseEnrollmentsRequest(
    int Page = 1,
    int PageSize = 10,
    EnrollmentStatus? Status = null,
    DateTime? EnrolledAfter = null,
    DateTime? EnrolledBefore = null
);

public record GenerateCertificateRequest(
    string? CertificateTemplate = null
);