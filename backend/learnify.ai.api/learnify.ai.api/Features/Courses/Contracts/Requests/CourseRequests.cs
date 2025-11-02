using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Courses;

public record CreateCourseRequest(
    string Title,
    string Description,
    string ShortDescription,
    int InstructorId,
    int CategoryId,
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
    string LearningObjectives
);

public record UpdateCourseRequest(
    string? Title = null,
    string? Description = null,
    string? ShortDescription = null,
    int? CategoryId = null,
    decimal? Price = null,
    decimal? DiscountPrice = null,
    int? DurationHours = null,
    CourseLevel? Level = null,
    string? Language = null,
    string? ThumbnailUrl = null,
    string? VideoPreviewUrl = null,
    bool? IsPublished = null,
    bool? IsFeatured = null,
    int? MaxStudents = null,
    string? Prerequisites = null,
    string? LearningObjectives = null
);

public record CourseFilterRequest(
    int? CategoryId = null,
    int? InstructorId = null,
    CourseLevel? Level = null,
    bool? IsPublished = null,
    bool? IsFeatured = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);
