namespace learnify.ai.api.Features.Courses.Core.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public int InstructorId { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int DurationHours { get; set; }
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public string Language { get; set; } = "English";
    public string? ThumbnailUrl { get; set; }
    public string? VideoPreviewUrl { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public int? MaxStudents { get; set; }
    public string? Prerequisites { get; set; }
    public string LearningObjectives { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public decimal GetEffectivePrice() => DiscountPrice ?? Price;
    public bool IsDiscounted() => DiscountPrice.HasValue && DiscountPrice < Price;
    public bool CanEnroll(int currentEnrollmentCount) => IsPublished && (MaxStudents == null || currentEnrollmentCount < MaxStudents);
}

public enum CourseLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}