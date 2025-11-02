using FluentValidation;
using learnify.ai.api.Common.Enums;
using learnify.ai.api.Common.Exceptions;
using learnify.ai.api.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Features.Users;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Gender Gender,
    RoleType Role = RoleType.Student,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null
) : ICommand<UserResponse>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
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
            .WithMessage("Bio cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.EmailExistsAsync(email, cancellationToken);
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.ToEntity();
        user.UserName = user.Email;
        
        var createResult = await _userManager.CreateAsync(user, request.Password);
        
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        var createdUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
        
        if (createdUser == null)
        {
            throw new InvalidOperationException("User was created but could not be retrieved");
        }

        var roleResult = await _userManager.AddToRoleAsync(createdUser, request.Role.ToString());

        if (!roleResult.Succeeded)
        {
            throw new RoleAssignmentException();
        }

        return createdUser.ToDto();
    }
}

