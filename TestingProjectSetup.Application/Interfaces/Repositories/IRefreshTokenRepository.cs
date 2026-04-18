using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for RefreshToken entities
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(string userId, string? reason = null, string? revokedByIp = null, CancellationToken cancellationToken = default);
}
