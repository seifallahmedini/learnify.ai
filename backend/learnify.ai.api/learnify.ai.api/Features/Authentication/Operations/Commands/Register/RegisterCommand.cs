using FluentValidation;
using learnify.ai.api.Common.Enums;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Authentication.Operations.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber = null
) : ICommand<RegisterResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandValidator(IUserRepository userRepository)
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

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Gender = Gender.Male, // Default, can be updated later
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        // Assign Student role by default
        var roleResult = await _userManager.AddToRoleAsync(user, RoleType.Student.ToString());

        if (!roleResult.Succeeded)
        {
            // If role assignment fails, remove the user
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign role: {errors}");
        }

        return new RegisterResponse(
            user.Id,
            user.Email ?? string.Empty,
            "Registration successful"
        );
    }
}

