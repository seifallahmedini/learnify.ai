using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users.Models;
using learnify.ai.api.Features.Users.Data;
using learnify.ai.api.Features.Users.Contracts.Requests;
using learnify.ai.api.Features.Users.Contracts.Responses;

namespace learnify.ai.api.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
    UserRole? Role = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<UserListResponse>;

public class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator()
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

public class GetUsersHandler : IRequestHandler<GetUsersQuery, UserListResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserListResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Apply filters
        var filteredUsers = allUsers.AsQueryable();

        if (request.Role.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.Role == request.Role.Value);
        }

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
                u.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = filteredUsers.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var users = filteredUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserSummaryResponse(
                u.Id,
                u.GetFullName(),
                u.Email,
                u.Role,
                u.IsActive,
                u.CreatedAt
            ));

        return new UserListResponse(
            users,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}