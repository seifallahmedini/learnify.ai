using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Users;

public record DeleteUserCommand(int Id) : ICommand<bool>;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
