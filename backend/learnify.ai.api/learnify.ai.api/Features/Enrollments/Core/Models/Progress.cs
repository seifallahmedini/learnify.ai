namespace learnify.ai.api.Features.Enrollments.Core.Models;

public class Progress
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int LessonId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletionDate { get; set; }
    public int TimeSpent { get; set; } = 0; // Time spent in minutes
    public DateTime LastAccessDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public void MarkAsCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletionDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AddTimeSpent(int minutes)
    {
        TimeSpent += minutes;
        LastAccessDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFormattedTimeSpent()
    {
        var hours = TimeSpent / 60;
        var minutes = TimeSpent % 60;
        return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
    }
}