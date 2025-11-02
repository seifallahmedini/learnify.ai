using FluentValidation;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Features.Authentication.Operations.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResponse?>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse?>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null || !user.IsActive)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return null;
        }

        // Generate JWT token
        var token = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);

        // Generate refresh token (in a real app, you'd store this in a database)
        var refreshToken = GenerateRefreshToken();

        var expiresAt = DateTime.UtcNow.AddHours(2);

        return new AuthResponse(
            token,
            refreshToken,
            expiresAt,
            new UserInfo(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                user.GetFullName(),
                user.IsActive
            )
        );
    }

    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
    }
}

