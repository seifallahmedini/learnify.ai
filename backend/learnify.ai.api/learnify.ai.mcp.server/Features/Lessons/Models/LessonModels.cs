using System.Text.Json.Serialization;
using System.Text.Json;
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

#region Extended Lesson Resources

/// <summary>
/// Extended lesson resources structure for comprehensive resource management
/// </summary>
public class ExtendedLessonResources
{
    [JsonPropertyName("essential_reading")]
    public List<EssentialReadingResource> EssentialReading { get; set; } = new();

    [JsonPropertyName("videos")]
    public List<VideoResourceItem> Videos { get; set; } = new();

    [JsonPropertyName("tools")]
    public List<ToolResource> Tools { get; set; } = new();

    [JsonPropertyName("research_papers")]
    public List<string> ResearchPapers { get; set; } = new();

    [JsonPropertyName("community")]
    public List<string> Community { get; set; } = new();

    [JsonPropertyName("practice_exercises")]
    public List<string> PracticeExercises { get; set; } = new();

    [JsonPropertyName("additional_resources")]
    public List<string> AdditionalResources { get; set; } = new();
}

/// <summary>
/// Essential reading resource
/// </summary>
public class EssentialReadingResource
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Video resource item
/// </summary>
public class VideoResourceItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    [JsonPropertyName("instructor")]
    public string? Instructor { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Tool/framework resource
/// </summary>
public class ToolResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// Helper class for managing extended lesson resources
/// </summary>
public static class ExtendedResourcesHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Creates a default empty extended resources structure
    /// </summary>
    public static ExtendedLessonResources CreateDefault()
    {
        return new ExtendedLessonResources();
    }

    /// <summary>
    /// Parses JSON string to ExtendedLessonResources, returns default if parsing fails
    /// </summary>
    public static ExtendedLessonResources ParseOrDefault(string? resourcesJson)
    {
        if (string.IsNullOrWhiteSpace(resourcesJson))
        {
            return CreateDefault();
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<ExtendedLessonResources>(resourcesJson, JsonOptions);
            return parsed ?? CreateDefault();
        }
        catch (JsonException)
        {
            return CreateDefault();
        }
    }

    /// <summary>
    /// Serializes ExtendedLessonResources to JSON string
    /// </summary>
    public static string Serialize(ExtendedLessonResources resources)
    {
        return JsonSerializer.Serialize(resources, JsonOptions);
    }

    /// <summary>
    /// Ensures resources string is in extended format
    /// </summary>
    public static string EnsureExtendedFormat(string? resourcesJson)
    {
        var resources = ParseOrDefault(resourcesJson);
        return Serialize(resources);
    }

    /// <summary>
    /// Gets total count of all resources
    /// </summary>
    public static int GetTotalResourceCount(ExtendedLessonResources resources)
    {
        int count = 0;
        count += resources.EssentialReading?.Count ?? 0;
        count += resources.Videos?.Count ?? 0;
        count += resources.Tools?.Count ?? 0;
        count += resources.ResearchPapers?.Count ?? 0;
        count += resources.Community?.Count ?? 0;
        count += resources.PracticeExercises?.Count ?? 0;
        count += resources.AdditionalResources?.Count ?? 0;
        return count;
    }
}

#endregion
