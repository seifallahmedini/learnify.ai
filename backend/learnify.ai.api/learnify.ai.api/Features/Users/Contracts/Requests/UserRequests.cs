using learnify.ai.api.Common.Enums;

namespace learnify.ai.api.Features.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    RoleType Role = RoleType.Student,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null,
    Gender Gender = Gender.Male
);

public record UpdateUserRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null,
    bool? IsActive = null,
    Gender? Gender = null
);

public record UserFilterRequest(
    RoleType? Role = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);
