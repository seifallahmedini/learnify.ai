using FluentValidation;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Features.Authentication.Operations.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token,
    string RefreshToken
) : ICommand<RefreshTokenResponse?>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse?>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RefreshTokenResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // In a production app, you should:
        // 1. Validate the refresh token against a database/cache
        // 2. Check if it's expired
        // 3. Verify it matches the user

        // For now, we'll parse the user ID from the token claims
        var principal = ValidateToken(request.Token);
        
        if (principal == null)
        {
            return null;
        }

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        
        if (user == null || !user.IsActive)
        {
            return null;
        }

        // Validate refresh token (simplified - in production, check against database)
        // TODO: Implement proper refresh token validation

        // Generate new tokens
        var newToken = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);
        var newRefreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(2);

        return new RefreshTokenResponse(
            newToken,
            newRefreshToken,
            expiresAt
        );
    }

    private static System.Security.Claims.ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var claims = jsonToken.Claims;
            
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            return new System.Security.Claims.ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }

    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
    }
}

