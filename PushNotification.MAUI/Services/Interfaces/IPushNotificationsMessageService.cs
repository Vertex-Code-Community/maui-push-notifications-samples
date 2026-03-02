namespace PushNotification.MAUI.Services.Interfaces;

public interface IPushNotificationsMessageService
{
    void Invoke(string title, string messgae);
    event Func<string, string, Task> OnNotificationCame;
}