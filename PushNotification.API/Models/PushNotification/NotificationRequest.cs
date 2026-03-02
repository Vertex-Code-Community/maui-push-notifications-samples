namespace PushNotification.API.Models.PushNotification;

public class NotificationRequest
{
    public string Title { get; set; } = "Notification";
    public string Text { get; set; }
    public string Url { get; set; }
    public string? TagExpression { get; set; }
}

