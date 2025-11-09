using System.Text.Json.Serialization;
using Learnify.Mcp.Server.Shared.Models;

namespace Learnify.Mcp.Server.Features.Lessons.Models;

/// <summary>
/// Lesson entity response model
/// </summary>
public record LessonModel(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("courseId")] int CourseId,
    [property: JsonPropertyName("courseTitle")] string CourseTitle,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("videoUrl")] string? VideoUrl,
    [property: JsonPropertyName("duration")] int Duration,
    [property: JsonPropertyName("orderIndex")] int OrderIndex,
    [property: JsonPropertyName("isFree")] bool IsFree,
    [property: JsonPropertyName("isPublished")] bool IsPublished,
    [property: JsonPropertyName("learningObjectives")] string? LearningObjectives,
    [property: JsonPropertyName("resources")] string? Resources,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTime UpdatedAt
)
{
    [JsonPropertyName("formattedDuration")]
    public string FormattedDuration
    {
        get
        {
            var hours = Duration / 60;
            var minutes = Duration % 60;
            return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
        }
    }
}

/// <summary>
/// Lesson summary for list views
/// </summary>
public record LessonSummaryModel(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("courseId")] int CourseId,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("duration")] int Duration,
    [property: JsonPropertyName("orderIndex")] int OrderIndex,
    [property: JsonPropertyName("isFree")] bool IsFree,
    [property: JsonPropertyName("isPublished")] bool IsPublished,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
);

/// <summary>
/// Course lessons response
/// </summary>
public record CourseLessonsModel(
    [property: JsonPropertyName("courseId")] int CourseId,
    [property: JsonPropertyName("courseTitle")] string CourseTitle,
    [property: JsonPropertyName("lessons")] IEnumerable<LessonSummaryModel> Lessons,
    [property: JsonPropertyName("totalLessons")] int TotalLessons,
    [property: JsonPropertyName("totalDuration")] int TotalDuration
);

/// <summary>
/// Lesson resources model
/// </summary>
public record LessonResourceModel(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("uploadedAt")] DateTime UploadedAt
);

/// <summary>
/// Lesson resources response
/// </summary>
public record LessonResourcesModel(
    [property: JsonPropertyName("lessonId")] int LessonId,
    [property: JsonPropertyName("lessonTitle")] string LessonTitle,
    [property: JsonPropertyName("resources")] IEnumerable<LessonResourceModel> Resources,
    [property: JsonPropertyName("resourceCount")] int ResourceCount
);

/// <summary>
/// Request models for lesson operations
/// </summary>
public record CreateLessonRequest(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("videoUrl")] string? VideoUrl,
    [property: JsonPropertyName("duration")] int Duration,
    [property: JsonPropertyName("isFree")] bool IsFree = false,
    [property: JsonPropertyName("isPublished")] bool IsPublished = false,
    [property: JsonPropertyName("learningObjectives")] string? LearningObjectives = null,
    [property: JsonPropertyName("resources")] string? Resources = null
);

public record UpdateLessonRequest(
    [property: JsonPropertyName("title")] string? Title = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("content")] string? Content = null,
    [property: JsonPropertyName("videoUrl")] string? VideoUrl = null,
    [property: JsonPropertyName("duration")] int? Duration = null,
    [property: JsonPropertyName("orderIndex")] int? OrderIndex = null,
    [property: JsonPropertyName("isFree")] bool? IsFree = null,
    [property: JsonPropertyName("isPublished")] bool? IsPublished = null,
    [property: JsonPropertyName("learningObjectives")] string? LearningObjectives = null,
    [property: JsonPropertyName("resources")] string? Resources = null
);

public record ReorderLessonRequest(
    [property: JsonPropertyName("newOrderIndex")] int NewOrderIndex
);

public record UploadVideoRequest(
    [property: JsonPropertyName("videoUrl")] string VideoUrl
);

public record UpdateContentRequest(
    [property: JsonPropertyName("content")] string Content
);

/// <summary>
/// Filter request for course lessons
/// </summary>
public record CourseLessonsFilterRequest(
    [property: JsonPropertyName("courseId")] int CourseId,
    [property: JsonPropertyName("isPublished")] bool? IsPublished = null
) : BaseFilterRequest();