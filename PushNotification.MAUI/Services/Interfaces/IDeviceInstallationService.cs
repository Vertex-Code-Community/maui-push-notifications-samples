using PushNotification.MAUI.Models.PushNotification;

namespace PushNotification.MAUI.Services.Interfaces;

public interface IDeviceInstallationService
{
    TaskCompletionSource<string?> DeviceTokenTcs { get; set; }
    bool NotificationsSupported { get; }
    string? GetDeviceId();
    Task<DeviceInstallation?> GetDeviceInstallationAsync(params List<string> tags);
}