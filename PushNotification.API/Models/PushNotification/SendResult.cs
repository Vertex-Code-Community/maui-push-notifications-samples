namespace PushNotification.API.Models.PushNotification;

public class SendResult
{
    public bool Success { get; init; }
    public string? TrackingId { get; init; }
    public string? Error { get; init; }

    public static SendResult Ok(string? trackingId = null) =>
        new() { Success = true, TrackingId = trackingId };

    public static SendResult Fail(string error) =>
        new() { Success = false, Error = error };
}
