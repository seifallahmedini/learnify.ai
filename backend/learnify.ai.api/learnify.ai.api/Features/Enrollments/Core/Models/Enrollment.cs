namespace learnify.ai.api.Features.Enrollments;

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal Progress { get; set; } = 0; // Progress percentage (0-100)
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public int? PaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool IsCompleted() => Status == EnrollmentStatus.Completed;
    public int GetProgressPercentage() => (int)Math.Round(Progress);
    public bool CanAccess() => Status == EnrollmentStatus.Active || Status == EnrollmentStatus.Completed;
}

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2,
    Dropped = 3,
    Suspended = 4
}