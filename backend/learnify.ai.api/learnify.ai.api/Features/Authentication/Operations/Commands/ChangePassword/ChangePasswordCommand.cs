using FluentValidation;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace learnify.ai.api.Features.Authentication.Operations.Commands.ChangePassword;

public record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand<ChangePasswordResponse>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must be different from current password");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        
        if (user == null || !user.IsActive)
        {
            return new ChangePasswordResponse(
                false,
                "User not found or inactive."
            );
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ChangePasswordResponse(
                false,
                $"Password change failed: {errors}"
            );
        }

        return new ChangePasswordResponse(
            true,
            "Password has been changed successfully."
        );
    }
}

