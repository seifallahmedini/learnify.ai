namespace learnify.ai.api.Features.Assessments;

public class Quiz
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int? LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? TimeLimit { get; set; } // Time limit in minutes
    public int PassingScore { get; set; } = 70; // Passing score percentage
    public int MaxAttempts { get; set; } = 3;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool HasTimeLimit() => TimeLimit.HasValue;
    public bool IsLessonQuiz() => LessonId.HasValue;
    public bool IsCourseQuiz() => !LessonId.HasValue;
}