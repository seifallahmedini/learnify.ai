using learnify.ai.api.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Domain.Entities;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfilePicture { get; set; }
    public Gender Gender { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public string GetFullName() => $"{FirstName} {LastName}";
}

