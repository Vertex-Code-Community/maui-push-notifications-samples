using Foundation;

namespace PushNotification.Maui.Services;

public static class NSDictionaryExtensions
{
    public static string? TryGetPayloadValue(this NSDictionary? userInfo, string key)
    {
        if (userInfo == null)
            return null;

        // Direct: "url"
        if (userInfo[new NSString(key)] is NSString direct)
            return direct.ToString();

        // Nested: { "data": { "url": "" } }
        if (userInfo[new NSString("data")] is NSDictionary data &&
            data[new NSString(key)] is NSString nested)
            return nested.ToString();

        // FCM-style: "gcm.notification.url"
        var gcmKey = $"gcm.notification.{key}";
        if (userInfo[new NSString(gcmKey)] is NSString gcm)
            return gcm.ToString();

        return null;
    }
}