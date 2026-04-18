using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TestingProjectSetup.Application.Interfaces.Services;
using TestingProjectSetup.Domain.Constants;
using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int RefreshTokenExpirationInDays => _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationInDays", 30);

    public string GenerateToken(ApplicationUser user)
    {
        var (token, _) = GenerateTokenWithExpiry(user, Array.Empty<string>());
        return token;
    }

    public (string Token, DateTime ExpiresAt) GenerateTokenWithExpiry(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60"));

        var claims = new Dictionary<string, object>
        {
            [AppClaimTypes.Subject] = user.Id,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [AppClaimTypes.Email] = user.Email ?? "",
            [AppClaimTypes.Name] = user.Name ?? user.UserName ?? user.PhoneNumber ?? "",
            [AppClaimTypes.PhoneNumber] = user.PhoneNumber ?? ""
        };

        foreach (var role in roles)
        {
            claims[$"http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] = role;
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return (token, expiresAt);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var handler = new JsonWebTokenHandler();

        var result = await handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        });

        return result.IsValid;
    }

    public string? GetUserIdFromToken(string token)
    {
        var handler = new JsonWebTokenHandler();

        try
        {
            var jwtToken = handler.ReadJsonWebToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
