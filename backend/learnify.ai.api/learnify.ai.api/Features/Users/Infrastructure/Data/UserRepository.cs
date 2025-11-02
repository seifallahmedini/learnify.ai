using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Users;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Gets all users with the Instructor role by querying Identity UserRoles table.
    /// Uses a single optimized query with joins for better performance.
    /// </summary>
    public async Task<IEnumerable<User>> GetInstructorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Join(
                _context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                _context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new { ur.user, role })
            .Where(x => x.role.Name == "Instructor")
            .Select(x => x.user)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all users with the Student role by querying Identity UserRoles table.
    /// Uses a single optimized query with joins for better performance.
    /// </summary>
    public async Task<IEnumerable<User>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Join(
                _context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                _context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new { ur.user, role })
            .Where(x => x.role.Name == "Student")
            .Select(x => x.user)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(u => u.IsActive, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(u => u.Email == email, cancellationToken);
    }
}
