using FluentValidation;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Domain.Entities;
using MediatR;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Authentication.Operations.Queries.GetCurrentUser;

public record GetCurrentUserQuery(int UserId) : IQuery<UserInfo?>;

public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserInfo?>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserInfo?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null || !user.IsActive)
        {
            return null;
        }

        return new UserInfo(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.GetFullName(),
            user.IsActive
        );
    }
}

