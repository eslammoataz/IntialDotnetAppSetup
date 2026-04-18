namespace TestingProjectSetup.Application.Interfaces.Services;

/// <summary>
/// SMS service interface
/// </summary>
public interface ISmsService
{
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);
}
