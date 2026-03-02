using PushNotification.MAUI.Models.PushNotification;
using PushNotification.MAUI.Services.Interfaces;
using UIKit;

namespace PushNotification.Maui.Services;

public class DeviceInstallationIOSService : IDeviceInstallationService
{
    const int SupportedVersionMajor = 14;
    const int SupportedVersionMinor = 0;

    public TaskCompletionSource<string?> DeviceTokenTcs { get; set; } = new();

    public bool NotificationsSupported =>
        UIDevice.CurrentDevice.CheckSystemVersion(SupportedVersionMajor, SupportedVersionMinor);

    public string? GetDeviceId() =>
        UIDevice.CurrentDevice.IdentifierForVendor.ToString();
    
    private bool _isDeviceIdFetching;

    public async Task<DeviceInstallation?> GetDeviceInstallationAsync(params List<string> tags)
    {
        if (!NotificationsSupported) return null;
        
        var deviceToken = await DeviceTokenTcs.Task;
        if (string.IsNullOrWhiteSpace(deviceToken)) return null;

        var deviceId = GetDeviceId();
        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var installation = new DeviceInstallation
            {
                InstallationId = deviceId,
                Platform = "apns",
                PushChannel = deviceToken,
                Tags = tags
            };

            return installation;
        };
        
        if (!_isDeviceIdFetching)
        {
            _isDeviceIdFetching = true;

            _ = Task.Run(async () =>
            {
                while (string.IsNullOrWhiteSpace(deviceId))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    var retryId = GetDeviceId();
                    if (!string.IsNullOrWhiteSpace(retryId))
                    {
                        deviceId = retryId;
                        Console.WriteLine($"[DeviceId] Retrieved after retry: {deviceId}");
                        break;
                    }
                }
            });
        }

        return null; 
    }

    // string GetNotificationsSupportError()
    // {
    //     if (!NotificationsSupported)
    //         return $"This app only supports notifications on iOS {SupportedVersionMajor}.{SupportedVersionMinor} and above. You are running {UIDevice.CurrentDevice.SystemVersion}.";
    //
    //     if (Token == null)
    //         return $"This app can support notifications but you must enable this in your settings.";
    //
    //     return "An error occurred preventing the use of push notifications";
    // }
}