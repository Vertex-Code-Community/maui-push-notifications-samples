using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;

namespace PushNotification.API.Services.Interfaces;

public interface INotificationService
{
    Task<SendResult> SendNotificationAsync(NotificationModel notificationModel,
        CancellationToken cancellationToken = default);
}

