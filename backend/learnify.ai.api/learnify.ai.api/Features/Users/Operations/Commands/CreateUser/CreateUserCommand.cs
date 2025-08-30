using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Users;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role = UserRole.Student,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null
) : ICommand<UserResponse>;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be valid")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters")
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email already exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage("Bio cannot exceed 1000 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.EmailExistsAsync(email, cancellationToken);
    }
}

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password), // TODO: Implement proper password hashing
            Role = request.Role,
            Bio = request.Bio,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        return new UserResponse(
            createdUser.Id,
            createdUser.FirstName,
            createdUser.LastName,
            createdUser.Email,
            createdUser.Role,
            createdUser.IsActive,
            createdUser.ProfilePicture,
            createdUser.Bio,
            createdUser.DateOfBirth,
            createdUser.PhoneNumber,
            createdUser.CreatedAt,
            createdUser.UpdatedAt
        );
    }

    private static string HashPassword(string password)
    {
        // TODO: Implement proper password hashing (e.g., BCrypt)
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}