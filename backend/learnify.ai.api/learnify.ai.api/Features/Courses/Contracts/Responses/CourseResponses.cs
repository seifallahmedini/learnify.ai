using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Courses;

public record CourseResponse(
    int Id,
    string Title,
    string Description,
    string ShortDescription,
    int InstructorId,
    string InstructorName,
    int CategoryId,
    string CategoryName,
    decimal Price,
    decimal? DiscountPrice,
    int DurationHours,
    CourseLevel Level,
    string Language,
    string? ThumbnailUrl,
    string? VideoPreviewUrl,
    bool IsPublished,
    bool IsFeatured,
    int? MaxStudents,
    string? Prerequisites,
    string LearningObjectives,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public decimal EffectivePrice => DiscountPrice ?? Price;
    public bool IsDiscounted => DiscountPrice.HasValue && DiscountPrice < Price;
}

public record CourseSummaryResponse(
    int Id,
    string Title,
    string ShortDescription,
    string InstructorName,
    string CategoryName,
    decimal Price,
    decimal? DiscountPrice,
    CourseLevel Level,
    string? ThumbnailUrl,
    bool IsPublished,
    bool IsFeatured,
    int TotalStudents,
    double AverageRating,
    DateTime CreatedAt
)
{
    public decimal EffectivePrice => DiscountPrice ?? Price;
    public bool IsDiscounted => DiscountPrice.HasValue && DiscountPrice < Price;
}

public record CourseListResponse(
    IEnumerable<CourseSummaryResponse> Courses,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record CourseDetailResponse(
    CourseResponse Course,
    IEnumerable<LessonSummaryResponse> Lessons,
    CourseStatisticsResponse Statistics
);

public record LessonSummaryResponse(
    int Id,
    string Title,
    string Description,
    int Duration,
    int OrderIndex,
    bool IsFree,
    bool IsPublished
);

public record CourseStatisticsResponse(
    int TotalStudents,
    int TotalLessons,
    int TotalDuration,
    double AverageRating,
    int ReviewCount
);
