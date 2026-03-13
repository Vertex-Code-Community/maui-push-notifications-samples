using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;
using PushNotification.API.Services.Interfaces;
using PushNotification.API.Utilites;

namespace PushNotification.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationHubService _notificationHubService;

    public NotificationService(INotificationHubService notificationHubService)
    {
        _notificationHubService = notificationHubService;
    }

    public async Task<SendResult> SendNotificationAsync(
        NotificationModel notificationModel,
        CancellationToken cancellationToken)
    {
        var tagExpression = NotificationTagExpressionBuilder.Build(notificationModel);

        var notificationRequest = new NotificationRequest
        {
            Title = notificationModel.Title,
            Url = notificationModel.UrlForRedirection ?? string.Empty,
            Text = notificationModel.Message,
            TagExpression = tagExpression
        };


        return await _notificationHubService.SendPushAsync(
            notificationModel.Id,
            notificationRequest,
            cancellationToken);
    }
}