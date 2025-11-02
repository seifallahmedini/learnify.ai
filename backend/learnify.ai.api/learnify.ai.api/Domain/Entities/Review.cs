namespace learnify.ai.api.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool IsValidRating() => Rating >= 1 && Rating <= 5;
    public bool HasComment() => !string.IsNullOrWhiteSpace(Comment);
    
    public void Approve()
    {
        IsApproved = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetStarRating() => new string('⭐', Rating) + new string('☆', 5 - Rating);
}

