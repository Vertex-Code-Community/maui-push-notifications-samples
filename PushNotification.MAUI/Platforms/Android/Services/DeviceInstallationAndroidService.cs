using Android.Gms.Common;
using Android.Provider;
using PushNotification.MAUI.Models.PushNotification;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Services;

public class DeviceInstallationAndroidService : IDeviceInstallationService
{
    public TaskCompletionSource<string?> DeviceTokenTcs { get; set; } = new();

    public bool NotificationsSupported
        => GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Platform.AppContext) == ConnectionResult.Success;

    public string? GetDeviceId()
        => Settings.Secure.GetString(Platform.AppContext.ContentResolver, Settings.Secure.AndroidId);

    public async Task<DeviceInstallation?> GetDeviceInstallationAsync(params List<string> tags)
    {
        if (!NotificationsSupported) return null;

        string? deviceToken = null;
        try
        {
            // Wait for device token with 10 second timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            deviceToken = await DeviceTokenTcs.Task.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("FCM token retrieval timed out after 10 seconds");
            throw new Exception("Unable to resolve token for FCMv1 - timeout waiting for token.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting FCM token: {ex.Message}");
            throw;
        }

        if (string.IsNullOrWhiteSpace(deviceToken)) 
            throw new Exception("Unable to resolve token for FCMv1.");
        
        var deviceId = GetDeviceId();
        if (string.IsNullOrWhiteSpace(deviceId)) return null;

        var installation = new DeviceInstallation
        {
            InstallationId = deviceId,
            Platform = "fcmv1",
            PushChannel = deviceToken,
            Tags = tags
        };

        return installation;
    }
}
