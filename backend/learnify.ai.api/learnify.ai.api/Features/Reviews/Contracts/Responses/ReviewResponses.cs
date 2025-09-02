namespace learnify.ai.api.Features.Reviews;

// Response DTOs for Review endpoints
public record ReviewResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    int UserId,
    string UserName,
    int Rating,
    string? Comment,
    bool IsApproved,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ReviewSummaryResponse(
    int Id,
    int CourseId,
    int UserId,
    string UserName,
    int Rating,
    string? Comment,
    bool IsApproved,
    DateTime CreatedAt
);

public record ReviewListResponse(
    IEnumerable<ReviewSummaryResponse> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record CourseReviewsResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<ReviewSummaryResponse> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    double AverageRating,
    RatingDistributionResponse RatingDistribution
);

public record RatingDistributionResponse(
    int OneStarCount,
    int TwoStarCount,
    int ThreeStarCount,
    int FourStarCount,
    int FiveStarCount,
    int TotalReviews
);

public record CourseRatingResponse(
    int CourseId,
    string CourseTitle,
    double AverageRating,
    int TotalReviews,
    RatingDistributionResponse RatingDistribution
);

public record CourseReviewStatsResponse(
    int CourseId,
    string CourseTitle,
    double AverageRating,
    int TotalReviews,
    int ApprovedReviews,
    int PendingReviews,
    double ApprovalRate,
    RatingDistributionResponse RatingDistribution,
    DateTime? LastReviewDate,
    DateTime CalculatedAt
);

public record UserReviewsResponse(
    int UserId,
    string UserName,
    IEnumerable<UserReviewSummaryResponse> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    double AverageRatingGiven
);

public record UserReviewSummaryResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    int Rating,
    string? Comment,
    bool IsApproved,
    DateTime CreatedAt
);

public record PendingReviewsResponse(
    IEnumerable<PendingReviewItemResponse> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record PendingReviewItemResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    int UserId,
    string UserName,
    int Rating,
    string? Comment,
    DateTime CreatedAt,
    int DaysPending
);

public record ReviewModerationResponse(
    int Id,
    bool IsApproved,
    DateTime UpdatedAt,
    string Message
);