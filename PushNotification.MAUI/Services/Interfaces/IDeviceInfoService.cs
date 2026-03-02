namespace PushNotification.MAUI.Services.Interfaces;

public interface IDeviceInfoService
{
    Task<string> GetDeviceId();
    Task<string> GetSystemVersion();
}