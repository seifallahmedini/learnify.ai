using learnify.ai.api.Features.Users.Models;

namespace learnify.ai.api.Features.Users.Contracts.Requests;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role = UserRole.Student,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null
);

public record UpdateUserRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null,
    bool? IsActive = null
);

public record UserFilterRequest(
    UserRole? Role = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);