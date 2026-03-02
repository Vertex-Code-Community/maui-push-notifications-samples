using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using PushNotification.API.Constants;
using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;
using PushNotification.API.Services.Interfaces;

namespace PushNotification.API.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly NotificationHubClient _hub;
    private readonly Dictionary<string, NotificationPlatform> _installationPlatform;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IOptions<NotificationHubOptions> options,
        ILogger<NotificationHubService> logger)
    {
        _logger = logger;
        
        _hub = NotificationHubClient.CreateClientFromConnectionString(
            options.Value.ConnectionString,
            options.Value.HubName);

        _installationPlatform = new Dictionary<string, NotificationPlatform>
        {
            { nameof(NotificationPlatform.Apns).ToLower(), NotificationPlatform.Apns },
            { nameof(NotificationPlatform.FcmV1).ToLower(), NotificationPlatform.FcmV1 }
        };
    }

    public async Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
            string.IsNullOrWhiteSpace(deviceInstallation?.Platform) ||
            string.IsNullOrWhiteSpace(deviceInstallation?.PushChannel))
            return false;

        var installation = new Installation
        {
            InstallationId = deviceInstallation.InstallationId,
            PushChannel = deviceInstallation.PushChannel,
            Tags = deviceInstallation.Tags
        };

        if (_installationPlatform.TryGetValue(deviceInstallation.Platform.ToLower(), out var platform))
            installation.Platform = platform;
        else
            return false;

        try
        {
            var saved = await _hub.GetInstallationAsync(installation.InstallationId, token);
            
            await _hub.CreateOrUpdateInstallationAsync(installation, token);
             saved = await _hub.GetInstallationAsync(installation.InstallationId, token);
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
        string platform,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(notificationRequest?.Text))
            return null;

        var androidPayload = PrepareNotificationPayload(
            PushTemplates.DataOnly.Android,
            notificationRequest.Title,
            notificationRequest.Text,
            notificationRequest.Url);

        var iOSPayload = PrepareNotificationPayload(
            PushTemplates.DataOnly.iOS,
            notificationRequest.Title,
            notificationRequest.Text,
            notificationRequest.Url);

        try
        {
            var tasks = new List<Task<NotificationOutcome[]>>
            {
                string.IsNullOrWhiteSpace(notificationRequest.TagExpression)
                    ? SendPlatformNotificationsAsync(androidPayload, iOSPayload, platform, token)
                    : SendPlatformNotificationsAsync(androidPayload, iOSPayload, notificationRequest.TagExpression, platform, token)
            };

            return (await Task.WhenAll(tasks)).ToList();
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

    private Task<NotificationOutcome[]> SendPlatformNotificationsAsync(
        string androidPayload,
        string iOSPayload,
        string platform,
        CancellationToken token)
    {
        var sendTasks = new List<Task<NotificationOutcome>>();

        switch (platform)
        {
            case Platform.Android:
                sendTasks.Add(_hub.SendFcmV1NativeNotificationAsync(androidPayload, token));
                break;

            case Platform.IOS:
                sendTasks.Add(_hub.SendAppleNativeNotificationAsync(iOSPayload, token));
                break;

            default:
                sendTasks.Add(_hub.SendFcmV1NativeNotificationAsync(androidPayload, token));
                sendTasks.Add(_hub.SendAppleNativeNotificationAsync(iOSPayload, token));
                break;
        }

        return Task.WhenAll(sendTasks);
    }

    private Task<NotificationOutcome[]> SendPlatformNotificationsAsync(
        string androidPayload,
        string iOSPayload,
        string tagExpression,
        string platform,
        CancellationToken token)
    {
        var sendTasks = new List<Task<NotificationOutcome>>();

        switch (platform)
        {
            case Platform.Android:
                sendTasks.Add(_hub.SendFcmV1NativeNotificationAsync(androidPayload, tagExpression, token));
                break;

            case Platform.IOS:
                sendTasks.Add(_hub.SendAppleNativeNotificationAsync(iOSPayload, tagExpression, token));
                break;

            default:
                sendTasks.Add(_hub.SendFcmV1NativeNotificationAsync(androidPayload, tagExpression, token));
                sendTasks.Add(_hub.SendAppleNativeNotificationAsync(iOSPayload, tagExpression, token));
                break;
        }

        return Task.WhenAll(sendTasks);
    }
}
