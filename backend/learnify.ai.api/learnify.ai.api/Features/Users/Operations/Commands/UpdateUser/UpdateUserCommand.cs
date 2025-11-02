using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Users;

public record UpdateUserCommand(
    int Id,
    string? FirstName = null,
    string? LastName = null,
    string? Bio = null,
    DateTime? DateOfBirth = null,
    string? PhoneNumber = null,
    bool? IsActive = null
) : ICommand<UserResponse?>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage("Bio cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse?>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            return null;

        request.UpdateEntity(user);

        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

        return updatedUser.ToResponse();
    }
}
