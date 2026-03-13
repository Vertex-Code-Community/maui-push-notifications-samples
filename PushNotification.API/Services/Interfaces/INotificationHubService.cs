using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;

namespace PushNotification.API.Services.Interfaces;

public interface INotificationHubService
{
    Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token);
    Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token);
    Task<SendResult> SendPushAsync(long notificationId, NotificationRequest notificationRequest, CancellationToken token);
}

