namespace learnify.ai.api.Features.Lessons;

public record ResourceItem(
    string Name,
    string Url,
    string Type // "download", "external", "code"
);

public record LessonResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    string Title,
    string Description,
    string Content,
    string? VideoUrl,
    int Duration,
    int OrderIndex,
    bool IsFree,
    bool IsPublished,
    string? LearningObjectives,
    string? Resources, // JSON string, will be parsed on frontend
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public string FormattedDuration => GetFormattedDuration();
    
    private string GetFormattedDuration()
    {
        var hours = Duration / 60;
        var minutes = Duration % 60;
        return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
    }
}

public record LessonSummaryResponse(
    int Id,
    int CourseId,
    string Title,
    string Description,
    int Duration,
    int OrderIndex,
    bool IsFree,
    bool IsPublished,
    DateTime CreatedAt
)
{
    public string FormattedDuration => GetFormattedDuration();
    
    private string GetFormattedDuration()
    {
        var hours = Duration / 60;
        var minutes = Duration % 60;
        return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
    }
}

public record LessonListResponse(
    IEnumerable<LessonSummaryResponse> Lessons,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record LessonNavigationResponse(
    LessonResponse? PreviousLesson,
    LessonResponse CurrentLesson,
    LessonResponse? NextLesson,
    int TotalLessons,
    int CurrentPosition
);
