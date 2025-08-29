using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Users.Models;

namespace learnify.ai.api.Features.Users.Data;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetInstructorsAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(u => u.Role == UserRole.Instructor || u.Role == UserRole.Admin, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(u => u.Role == UserRole.Student, cancellationToken);
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