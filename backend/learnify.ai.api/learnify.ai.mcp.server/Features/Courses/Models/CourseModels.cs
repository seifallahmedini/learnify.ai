using System.ComponentModel.DataAnnotations;

namespace Learnify.Mcp.Server.Features.Courses.Models;

#region Course Models

/// <summary>
/// Course model representing the main course entity
/// </summary>
public record CourseModel(
    int Id,
    string Title,
    string Description,
    string? ShortDescription,
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
    string? LearningObjectives,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Course summary model for lightweight operations
/// </summary>
public record CourseSummaryModel(
    int Id,
    string Title,
    string? ShortDescription,
    int InstructorId,
    string InstructorName,
    decimal Price,
    decimal? DiscountPrice,
    CourseLevel Level,
    string Language,
    bool IsPublished,
    bool IsFeatured,
    DateTime CreatedAt
);

/// <summary>
/// Course list response model with pagination
/// </summary>
public record CourseListResponse(
    IEnumerable<CourseModel> Courses,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

#endregion

#region Request Models

/// <summary>
/// Request model for creating a new course
/// </summary>
public record CreateCourseRequest(
    [Required] string Title,
    [Required] string Description,
    string? ShortDescription,
    [Required] int InstructorId,
    [Required] int CategoryId,
    [Required] decimal Price,
    decimal? DiscountPrice,
    [Required] int DurationHours,
    CourseLevel Level,
    [Required] string Language,
    string? ThumbnailUrl,
    string? VideoPreviewUrl,
    bool IsPublished = false,
    bool IsFeatured = false,
    int? MaxStudents = null,
    string? Prerequisites = null,
    string? LearningObjectives = null
);

/// <summary>
/// Request model for updating an existing course
/// </summary>
public record UpdateCourseRequest(
    string? Title,
    string? Description,
    string? ShortDescription,
    int? CategoryId,
    decimal? Price,
    decimal? DiscountPrice,
    int? DurationHours,
    CourseLevel? Level,
    string? Language,
    string? ThumbnailUrl,
    string? VideoPreviewUrl,
    bool? IsPublished,
    bool? IsFeatured,
    int? MaxStudents,
    string? Prerequisites,
    string? LearningObjectives
);

/// <summary>
/// Request model for filtering courses
/// </summary>
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

/// <summary>
/// Request model for course enrollment operations
/// </summary>
public record CourseEnrollmentRequest(
    int CourseId,
    int StudentId
);

/// <summary>
/// Request model for publishing/unpublishing courses
/// </summary>
public record CoursePublishRequest(
    bool IsPublished
);

/// <summary>
/// Request model for featuring/unfeaturing courses
/// </summary>
public record CourseFeaturedRequest(
    bool IsFeatured
);

#endregion

#region Response Models

/// <summary>
/// Course statistics response model
/// </summary>
public record CourseStatsResponse(
    int CourseId,
    int TotalEnrollments,
    int ActiveEnrollments,
    int CompletedEnrollments,
    decimal AverageRating,
    int TotalReviews,
    int TotalLessons,
    decimal Revenue
);

/// <summary>
/// Course enrollment response model
/// </summary>
public record CourseEnrollmentResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    int StudentId,
    string StudentName,
    DateTime EnrolledAt,
    DateTime? CompletedAt,
    decimal Progress,
    bool IsActive
);

/// <summary>
/// Course enrollments list response
/// </summary>
public record CourseEnrollmentsResponse(
    IEnumerable<CourseEnrollmentResponse> Enrollments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

#endregion

#region Enums

/// <summary>
/// Course difficulty levels
/// </summary>
public enum CourseLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

#endregion