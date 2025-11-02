using learnify.ai.api.Common.Enums;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Users;

public static class UserMapper
{
    // Core -> DTO (Contracts)
    public static UserResponse ToDto(this User user) => user.ToResponse();

    public static IEnumerable<UserResponse> ToDto(this IEnumerable<User> users)
        => users.Select(u => u.ToDto()).ToList();

    // Contracts -> Core (from HTTP request contracts)
    public static User ToEntity(this CreateUserRequest request)
    {
        return new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // Commands -> Core (from MediatR commands)
    public static User ToEntity(this CreateUserCommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        return new User
        {
            UserName = command.Email,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Bio = command.Bio,
            DateOfBirth = command.DateOfBirth,
            PhoneNumber = command.PhoneNumber,
            Gender = command.Gender,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateUserCommand command, User entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        if (!string.IsNullOrWhiteSpace(command.FirstName))
            entity.FirstName = command.FirstName;
        if (!string.IsNullOrWhiteSpace(command.LastName))
            entity.LastName = command.LastName;
        if (command.Bio is not null)
            entity.Bio = command.Bio;
        if (command.DateOfBirth.HasValue)
            entity.DateOfBirth = command.DateOfBirth.Value;
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
            entity.PhoneNumber = command.PhoneNumber;
        if (command.IsActive.HasValue)
            entity.IsActive = command.IsActive.Value;

        entity.UpdatedAt = DateTime.UtcNow;
    }

    // Core -> Contracts (kept for backward compatibility with existing code)
    public static UserResponse ToResponse(this User entity)
    {
        return new UserResponse(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Email ?? string.Empty,
            //entity.Role,
            entity.IsActive,
            entity.ProfilePicture,
            entity.Bio,
            entity.DateOfBirth,
            entity.PhoneNumber,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.Gender
        );
    }

    public static UserSummaryResponse ToSummary(this User entity)
    {
        return new UserSummaryResponse(
            entity.Id,
            entity.GetFullName(),
            entity.Email ?? string.Empty,
            //entity.Role,
            entity.IsActive,
            entity.CreatedAt
        );
    }

    public static IEnumerable<UserSummaryResponse> ToSummaries(this IEnumerable<User> entities)
        => entities.Select(e => e.ToSummary());

    // Apply partial updates from UpdateUserRequest to an existing User entity (legacy route)
    public static void Apply(this UpdateUserRequest request, User entity)
    {
        if (!string.IsNullOrWhiteSpace(request.FirstName))
            entity.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName))
            entity.LastName = request.LastName;
        if (request.Bio is not null)
            entity.Bio = request.Bio;
        if (request.DateOfBirth.HasValue)
            entity.DateOfBirth = request.DateOfBirth.Value;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            entity.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue)
            entity.IsActive = request.IsActive.Value;

        entity.UpdatedAt = DateTime.UtcNow;
    }
}
