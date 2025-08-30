using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Users;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetInstructorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetStudentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}