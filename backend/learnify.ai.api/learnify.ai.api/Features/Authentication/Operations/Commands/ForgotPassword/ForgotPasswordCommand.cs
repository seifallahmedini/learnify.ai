using FluentValidation;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Features.Authentication.Operations.Commands.ForgotPassword;

public record ForgotPasswordCommand(
    string Email
) : ICommand<ForgotPasswordResponse>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be valid");
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly UserManager<User> _userManager;

    public ForgotPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        // Always return success for security (prevent email enumeration)
        if (user == null || !user.IsActive)
        {
            return new ForgotPasswordResponse(
                true,
                "If an account with that email exists, a password reset link has been sent."
            );
        }

        // Generate password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // In a real application, you would:
        // 1. Store the token in a database with expiration
        // 2. Send an email with the reset link
        // 3. The reset link would contain the token

        // For now, we'll just return success
        // TODO: Implement email service to send reset link
        
        return new ForgotPasswordResponse(
            true,
            "If an account with that email exists, a password reset link has been sent."
        );
    }
}

