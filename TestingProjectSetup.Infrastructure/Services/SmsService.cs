using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Interfaces.Services;

namespace TestingProjectSetup.Infrastructure.Services;

/// <summary>
/// Stub implementation of SMS service - replace with real provider (Twilio, AWS SNS, etc.)
/// </summary>
public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SMS OTP sent to {Phone}: {Otp}", phoneNumber, otp);
        return Task.CompletedTask;
    }
}
