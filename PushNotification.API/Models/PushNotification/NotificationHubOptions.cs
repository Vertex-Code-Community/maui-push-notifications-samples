namespace PushNotification.API.Models.PushNotification;

public class NotificationHubOptions
{
    public string ConnectionString { get; set; }
    public string HubName { get; set; } = null!;
}
