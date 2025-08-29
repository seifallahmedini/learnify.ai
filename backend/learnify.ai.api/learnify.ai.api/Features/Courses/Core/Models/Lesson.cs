namespace learnify.ai.api.Features.Courses.Core.Models;

public class Lesson
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int Duration { get; set; } // Duration in minutes
    public int OrderIndex { get; set; }
    public bool IsFree { get; set; } = false;
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public string GetFormattedDuration()
    {
        var hours = Duration / 60;
        var minutes = Duration % 60;
        return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
    }
}