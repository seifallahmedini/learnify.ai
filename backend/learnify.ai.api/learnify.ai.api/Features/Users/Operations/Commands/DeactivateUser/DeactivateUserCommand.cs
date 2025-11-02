using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Users;

public record DeactivateUserCommand(int Id) : ICommand<UserResponse?>;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponse?>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            return null;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

        return updatedUser.ToResponse();
    }
}

