using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;

    public JwtTokenService(IConfiguration config, UserManager<User> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(User user, CancellationToken ct = default)
    {
        var key = _config["Jwt:Key"] ?? "dev_super_secret_key_change_me_minimum_32_bytes_long_key_for_security";
        var issuer = _config["Jwt:Issuer"] ?? "learnify-api";
        var audience = _config["Jwt:Audience"] ?? "learnify-clients";
        
        // Ensure key is at least 32 bytes (256 bits) for HS256
        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length < 32)
        {
            // Pad the key to 32 bytes or use SHA256 hash of the key
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            keyBytes = sha256.ComputeHash(keyBytes);
        }
        
        var signingKey = new SymmetricSecurityKey(keyBytes);

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var username = user.Email ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
