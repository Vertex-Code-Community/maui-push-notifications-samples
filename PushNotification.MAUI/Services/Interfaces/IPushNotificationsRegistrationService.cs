namespace PushNotification.MAUI.Services.Interfaces;

public interface IPushNotificationsRegistrationService
{
    Task DeregisterDeviceAsync();
    Task RegisterDeviceAsync();
}