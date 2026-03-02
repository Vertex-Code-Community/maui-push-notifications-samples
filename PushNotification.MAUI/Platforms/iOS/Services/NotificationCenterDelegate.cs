
using UserNotifications;

namespace PushNotification.Maui.Services
{
    public class NotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        const string UNNotificationDefaultActionIdentifier = "com.apple.UNNotificationDefaultActionIdentifier";
        const string UNNotificationDismissActionIdentifier = "com.apple.UNNotificationDismissActionIdentifier";

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Show it even in foreground (for testing)
            completionHandler(UNNotificationPresentationOptions.Alert |
                              UNNotificationPresentationOptions.Badge |
                              UNNotificationPresentationOptions.Sound);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            try
            {
                var actionId = response.ActionIdentifier;

                // Ignore explicit dismiss
                if (actionId == UNNotificationDismissActionIdentifier)
                {
                    completionHandler();
                    return;
                }

                var userInfo = response.Notification?.Request?.Content?.UserInfo;
                var urlString = userInfo.TryGetPayloadValue("url");

                var isDefaultTap = actionId == UNNotificationDefaultActionIdentifier;
                var isOpenAction = actionId == "OPEN_IN_BROWSER";

                if ((isDefaultTap || isOpenAction) &&
                    !string.IsNullOrWhiteSpace(urlString) &&
                    Uri.TryCreate(urlString, UriKind.Absolute, out var uri))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[DEMO] Failed to open URL: {ex}");
                        }
                    });
                }
            }
            finally
            {
                completionHandler();
            }
        }
    }
}
