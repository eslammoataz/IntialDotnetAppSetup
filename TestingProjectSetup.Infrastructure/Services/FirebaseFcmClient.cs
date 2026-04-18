using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Interfaces.Services;
using TestingProjectSetup.Domain.Enums;
using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Infrastructure.Services;

/// <summary>
/// Stub implementation of Firebase Cloud Messaging client - replace with real Firebase SDK
/// </summary>
public class FirebaseFcmClient : IFcmClient
{
    private readonly ILogger<FirebaseFcmClient> _logger;

    public FirebaseFcmClient(ILogger<FirebaseFcmClient> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string token, Notification notification, NotificationType type, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FCM push notification sent to {Token}: [{Type}] {Title} - {Body}",
            token, type, notification.Title, notification.Body);
        return Task.CompletedTask;
    }
}
