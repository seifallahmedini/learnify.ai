namespace learnify.ai.api.Common.Abstractions;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(User user, CancellationToken cancellationToken = default);
}

