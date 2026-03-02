using Android.App;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Firebase.Messaging;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IOnSuccessListener
{
    IPushNotificationsMessageService _notificationMessageService;
    IDeviceInstallationService _deviceInstallationService;
    
    IPushNotificationsMessageService NotificationMessageService =>
        _notificationMessageService ?? (_notificationMessageService = IPlatformApplication.Current.Services.GetService<IPushNotificationsMessageService>());

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>());

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is null) return;

        var token = result.ToString();
        if (!string.IsNullOrWhiteSpace(token))
            DeviceInstallationService.DeviceTokenTcs.TrySetResult(token);
    }
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        if (DeviceInstallationService.NotificationsSupported)
            FirebaseMessaging.Instance.GetToken().AddOnSuccessListener(this);
    }
}