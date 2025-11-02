namespace learnify.ai.api.Features.Lessons;

public record CreateLessonRequest(
    string Title,
    string Description,
    string Content,
    string? VideoUrl,
    int Duration,
    bool IsFree = false,
    bool IsPublished = false
);

public record UpdateLessonRequest(
    string? Title = null,
    string? Description = null,
    string? Content = null,
    string? VideoUrl = null,
    int? Duration = null,
    int? OrderIndex = null,
    bool? IsFree = null,
    bool? IsPublished = null
);

public record ReorderLessonRequest(
    int NewOrderIndex
);

public record UploadVideoRequest(
    string VideoUrl
);

public record UpdateContentRequest(
    string Content
);
