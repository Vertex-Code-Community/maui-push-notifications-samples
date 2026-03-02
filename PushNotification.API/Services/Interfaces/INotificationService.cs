using Microsoft.Azure.NotificationHubs;
using PushNotification.API.Models;

namespace PushNotification.API.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationOutcome []>?> RequestSendNotification(NotificationModel notificationModel,
        CancellationToken cancellationToken = default);
}

