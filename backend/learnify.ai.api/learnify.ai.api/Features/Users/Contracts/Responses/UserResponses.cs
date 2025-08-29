using learnify.ai.api.Features.Users.Models;

namespace learnify.ai.api.Features.Users.Contracts.Responses;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    bool IsActive,
    string? ProfilePicture,
    string? Bio,
    DateTime? DateOfBirth,
    string? PhoneNumber,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record UserSummaryResponse(
    int Id,
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt
);

public record UserListResponse(
    IEnumerable<UserSummaryResponse> Users,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}