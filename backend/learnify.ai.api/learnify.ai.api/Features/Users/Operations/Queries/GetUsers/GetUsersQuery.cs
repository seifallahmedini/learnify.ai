using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Enums;

namespace learnify.ai.api.Features.Users;

public record GetUsersQuery(
    RoleType? Role = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<UserListResponse>;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, UserListResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserListResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Apply filters
        var filteredUsers = allUsers.AsQueryable();

        if (request.IsActive.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredUsers = filteredUsers.Where(u => 
                u.FirstName.ToLower().Contains(searchTerm) ||
                u.LastName.ToLower().Contains(searchTerm) ||
                (u.Email ?? string.Empty).ToLower().Contains(searchTerm));
        }

        var totalCount = filteredUsers.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var users = filteredUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => u.ToSummary());

        return new UserListResponse(
            users,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
