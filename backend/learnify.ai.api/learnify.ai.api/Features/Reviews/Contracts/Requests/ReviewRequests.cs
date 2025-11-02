namespace learnify.ai.api.Features.Reviews;

// Request DTOs for Review endpoints
public record CreateReviewRequest(
    int CourseId,
    int UserId,
    int Rating,
    string? Comment = null
);

public record UpdateReviewRequest(
    int? Rating = null,
    string? Comment = null
);

public record GetReviewsRequest(
    int? CourseId = null,
    int? UserId = null,
    bool? IsApproved = null,
    int Page = 1,
    int PageSize = 10
);

public record GetCourseReviewsRequest(
    int Page = 1,
    int PageSize = 10,
    int? Rating = null,
    bool? IsApproved = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
);

public record GetUserReviewsRequest(
    int Page = 1,
    int PageSize = 10,
    bool? IsApproved = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
);

public record GetPendingReviewsRequest(
    int Page = 1,
    int PageSize = 10,
    DateTime? FromDate = null,
    DateTime? ToDate = null
);
