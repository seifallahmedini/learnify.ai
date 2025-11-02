namespace learnify.ai.api.Domain.Entities;

public class QuizAttempt
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpent { get; set; } // Time spent in minutes
    public bool IsPassed { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool IsCompleted() => CompletedAt.HasValue;
    public int GetScorePercentage() => MaxScore > 0 ? (int)Math.Round((double)Score / MaxScore * 100) : 0;
    public bool IsInProgress() => !CompletedAt.HasValue;
    
    public void CompleteAttempt(int finalScore, int passingScore)
    {
        Score = finalScore;
        CompletedAt = DateTime.UtcNow;
        TimeSpent = (int)(CompletedAt.Value - StartedAt).TotalMinutes;
        IsPassed = GetScorePercentage() >= passingScore;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFormattedTimeSpent()
    {
        var hours = TimeSpent / 60;
        var minutes = TimeSpent % 60;
        return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
    }
}

