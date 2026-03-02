
using PushNotification.MAUI.Services.Interfaces;
#if IOS
using UserNotifications;
using UIKit;
using Foundation;
#endif

namespace PushNotification.MAUI.Services;

public class PushNotificationPermissionService : IPushNotificationPermissionService
{
    
    public async Task<PermissionStatus> CheckStatusAsync()
    {
#if IOS
        var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();

        return settings.AuthorizationStatus switch
        {
            UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
            UNAuthorizationStatus.Provisional => PermissionStatus.Granted,
            UNAuthorizationStatus.Denied => PermissionStatus.Denied,
            UNAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
            _ => PermissionStatus.Unknown
        };
#elif ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        }
        
        return PermissionStatus.Granted;
#else
        return PermissionStatus.Unknown;
#endif
    }
    
    public async Task<bool> RequestAndRegisterAsync()
    {
#if ANDROID
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            return true;
        }

#elif IOS
        try
        {
            var (granted, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound);

            if (granted)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UIApplication.SharedApplication.RegisterForRemoteNotifications();
                });
            }

            return granted;
        }
        catch
        {
            return false;
        }
#else
        return await Task.FromResult(false);
#endif
    }
}