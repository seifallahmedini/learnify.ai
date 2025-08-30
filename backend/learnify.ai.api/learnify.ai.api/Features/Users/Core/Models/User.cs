namespace learnify.ai.api.Features.Users;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsActive { get; set; } = true;
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public string GetFullName() => $"{FirstName} {LastName}";
    public bool IsInstructor() => Role == UserRole.Instructor || Role == UserRole.Admin;
    public bool IsStudent() => Role == UserRole.Student;
}

public enum UserRole
{
    Student = 1,
    Instructor = 2,
    Admin = 3
}