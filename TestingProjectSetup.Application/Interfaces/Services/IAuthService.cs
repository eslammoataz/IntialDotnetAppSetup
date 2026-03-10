using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;

namespace TestingProjectSetup.Application.Interfaces.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<Result<string>> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> ValidateOtpAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
