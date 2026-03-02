namespace PushNotification.API.Models;

public class NotificationModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string? UrlForRedirection { get; set; }
    public string Platform { get; set; }
    public Dictionary<NotificationTagsType, HashSet<string>> Tags { get; set; } = new();
}

public enum NotificationTagsType
{
    User,
    VersionOwner,
    OperationSystem // IOS/ Android / All
}