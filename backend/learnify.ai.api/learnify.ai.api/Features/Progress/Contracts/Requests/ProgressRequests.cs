namespace learnify.ai.api.Features.Progress;

// Request DTOs for Progress endpoints
public record MarkLessonCompleteRequest(
    int EnrollmentId
);

public record TrackTimeSpentRequest(
    int EnrollmentId,
    int TimeSpentMinutes
);

public record GetLessonProgressRequest(
    int? EnrollmentId = null
);

public record GetUserLearningStatsRequest(
    DateTime? StartDate = null,
    DateTime? EndDate = null
);

public record GetCourseProgressStatsRequest(
    DateTime? StartDate = null,
    DateTime? EndDate = null
);

public record GetProgressDashboardRequest(
    int? Limit = 10
);