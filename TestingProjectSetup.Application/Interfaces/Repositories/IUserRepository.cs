using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Interfaces.Repositories;

/// <summary>
/// User repository interface
/// </summary>
public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task SaveTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<string?> GetTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task RemoveTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
}
