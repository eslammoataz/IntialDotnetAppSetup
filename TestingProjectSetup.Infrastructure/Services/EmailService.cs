using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Interfaces.Services;

namespace TestingProjectSetup.Infrastructure.Services;

/// <summary>
/// Stub implementation of email service - replace with real provider (SendGrid, AWS SES, etc.)
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email OTP sent to {Email}: {Otp}", toEmail, otp);
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string toEmail, string name, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Welcome email to {Email} for {Name} with temp password: {Password}", toEmail, name, temporaryPassword);
        return Task.CompletedTask;
    }
}
