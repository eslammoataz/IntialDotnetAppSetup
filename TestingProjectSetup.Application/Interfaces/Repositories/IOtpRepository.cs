using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Interfaces.Repositories;

/// <summary>
/// OTP repository interface
/// </summary>
public interface IOtpRepository : IRepository<Otp>
{
    Task<Otp?> GetValidOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task InvalidateOtpAsync(Otp otp, CancellationToken cancellationToken = default);
    Task InvalidateAllOtpsForPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
