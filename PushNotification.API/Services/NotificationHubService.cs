using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;
using PushNotification.API.Services.Interfaces;

namespace PushNotification.API.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly NotificationHubClient _hub;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IOptions<NotificationHubOptions> options,
        ILogger<NotificationHubService> logger)
    {
        _logger = logger;

        _hub = NotificationHubClient.CreateClientFromConnectionString(
            options.Value.ConnectionString,
            options.Value.HubName);
    }

    public async Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
            string.IsNullOrWhiteSpace(deviceInstallation?.Platform) ||
            string.IsNullOrWhiteSpace(deviceInstallation?.PushChannel))
            return false;

        if (!deviceInstallation.Platform.Equals("fcmv1", StringComparison.OrdinalIgnoreCase))
            return false;

        var installation = new Installation
        {
            InstallationId = deviceInstallation.InstallationId,
            PushChannel = deviceInstallation.PushChannel,
            Platform = NotificationPlatform.FcmV1,
            Tags = deviceInstallation.Tags
        };

        try
        {
            await _hub.CreateOrUpdateInstallationAsync(installation, token);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CreateOrUpdateInstallationAsync failed");
            return false;
        }
    }

    public async Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(installationId))
            return false;

        try
        {
            await _hub.DeleteInstallationAsync(installationId, token);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "DeleteInstallationByIdAsync failed");
            return false;
        }
    }

    public async Task<List<NotificationOutcome[]>?> RequestNotificationAsync(
        long notificationId,
        NotificationRequest notificationRequest,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(notificationRequest?.Text))
            return null;

        var payload = PrepareNotificationPayload(
            PushTemplates.DataOnly.Android,
            notificationRequest.Title,
            notificationRequest.Text,
            notificationRequest.Url);

        try
        {
            NotificationOutcome[] outcome;

            if (string.IsNullOrWhiteSpace(notificationRequest.TagExpression))
                outcome = [await _hub.SendFcmV1NativeNotificationAsync(payload, token)];
            else
                outcome = [await _hub.SendFcmV1NativeNotificationAsync(payload, notificationRequest.TagExpression, token)];

            return [outcome];
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending notification");
            return null;
        }
    }

    private string PrepareNotificationPayload(string template, string title, string text, string url) => template
        .Replace("$(titlePlaceholder)", title ?? "", StringComparison.InvariantCulture)
        .Replace("$(messagePlaceholder)", text ?? "", StringComparison.InvariantCulture)
        .Replace("$(urlPlaceholder)", url ?? "", StringComparison.InvariantCulture);
}
