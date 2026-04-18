namespace TestingProjectSetup.Application.Interfaces.Services;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string toEmail, string name, string temporaryPassword, CancellationToken cancellationToken = default);
}
