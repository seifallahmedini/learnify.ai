using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users.Data;
using learnify.ai.api.Features.Users.Contracts.Responses;

namespace learnify.ai.api.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IQuery<UserResponse?>;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            return null;

        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.IsActive,
            user.ProfilePicture,
            user.Bio,
            user.DateOfBirth,
            user.PhoneNumber,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}