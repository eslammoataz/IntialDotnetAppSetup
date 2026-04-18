using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Interfaces.Services;

/// <summary>
/// Token service interface for JWT operations
/// </summary>
public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
    (string Token, DateTime ExpiresAt) GenerateTokenWithExpiry(ApplicationUser user, IList<string> roles);
    Task<bool> ValidateTokenAsync(string token);
    string? GetUserIdFromToken(string token);

    // Refresh token support
    string GenerateRefreshToken();
    string HashToken(string token);
    int RefreshTokenExpirationInDays { get; }
}
