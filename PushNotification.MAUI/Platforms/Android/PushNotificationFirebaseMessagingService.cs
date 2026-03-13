using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Firebase.Messaging;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
    const string CHANNEL_ID = "push_main";
    const string CHANNEL_NAME = "General";

    IDeviceInstallationService? _deviceInstallationService;

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ??= IPlatformApplication.Current!.Services
            .GetRequiredService<IDeviceInstallationService>();

    public override void OnNewToken(string token)
        => DeviceInstallationService.DeviceTokenTcs.TrySetResult(token);

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        var data = message.Data;

        data.TryGetValue("title", out var title);
        data.TryGetValue("body", out var body);

        title ??= message.GetNotification()?.Title ?? "Notification";
        body  ??= message.GetNotification()?.Body  ?? string.Empty;

        DisplayNotification(title, body);
    }

    private void DisplayNotification(string title, string body)
    {
        InitChannel();

        var intent = new Intent(this, typeof(MainActivity))
            .AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        var pending = PendingIntent.GetActivity(this, 0, intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var smallIcon = Resources!.GetIdentifier("appicon", "mipmap", PackageName);
        if (smallIcon == 0)
            smallIcon = Resources.GetIdentifier("ic_launcher", "mipmap", PackageName);

        var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetSmallIcon(smallIcon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(body))
            .SetContentIntent(pending)
            .SetAutoCancel(true)
            .Build();

        var id = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0x7FFFFFFF);
        NotificationManagerCompat.From(this)?.Notify(id, notification);
    }

    private void InitChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

        var mgr = (NotificationManager)GetSystemService(NotificationService)!;
        if (mgr.GetNotificationChannel(CHANNEL_ID) != null) return;

        mgr.CreateNotificationChannel(
            new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Default)
            {
                Description = "General push notifications"
            });
    }
}
