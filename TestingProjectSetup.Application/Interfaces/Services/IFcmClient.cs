using TestingProjectSetup.Domain.Enums;
using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Interfaces.Services;

/// <summary>
/// Firebase Cloud Messaging client interface
/// </summary>
public interface IFcmClient
{
    Task SendAsync(string token, Notification notification, NotificationType type, CancellationToken cancellationToken = default);
}
