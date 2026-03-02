namespace PushNotification.MAUI.Services.Interfaces;

public interface IPushNotificationPermissionService
{
    Task<bool> RequestAndRegisterAsync();
    Task<PermissionStatus> CheckStatusAsync();
}