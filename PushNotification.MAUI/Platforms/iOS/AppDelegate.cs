using System.Diagnostics;
using Foundation;
using PushNotification.Maui.Services;
using PushNotification.MAUI.Services.Interfaces;
using UIKit;
using UserNotifications;


namespace PushNotification.MAUI;

 [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        IPushNotificationsRegistrationService _pushNotificationsRegistrationService;
        IDeviceInstallationService _deviceInstallationService;
        
        IPushNotificationsRegistrationService PushNotificationsRegistrationService =>
            _pushNotificationsRegistrationService ?? (_pushNotificationsRegistrationService = IPlatformApplication.Current.Services.GetService<IPushNotificationsRegistrationService>());

        IDeviceInstallationService DeviceInstallationService =>
            _deviceInstallationService ?? (_deviceInstallationService = IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>());
       
        protected override MauiApp CreateMauiApp()
        {
            var mauiApp = MauiProgram.CreateMauiApp();
            return mauiApp;
        }
        
        Task CompleteRegistrationAsync(NSData deviceToken)
        {
            var deviceTokeStr = deviceToken.ToHexString();
            DeviceInstallationService.DeviceTokenTcs.TrySetResult(deviceTokeStr);
            return Task.CompletedTask;
        }
        
        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            CompleteRegistrationAsync(deviceToken)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted)
                        throw task.Exception;
                });
        }
        
        [Export("application:didReceiveRemoteNotification:")]
        public void ReceivedRemoteNotification(UIApplication application, NSDictionary sourceUserInfo)
        {
            var titleString = sourceUserInfo.TryGetPayloadValue("title");
            var bodyString = sourceUserInfo.TryGetPayloadValue("body");
            var urlString = sourceUserInfo.TryGetPayloadValue("url");
            
            var content = new UNMutableNotificationContent
            {
                Title = titleString ?? string.Empty,
                Body = bodyString ?? string.Empty,
                Sound = UNNotificationSound.Default
            };

            var userInfo = new NSMutableDictionary();

            if (!string.IsNullOrWhiteSpace(titleString))
                userInfo.SetValueForKey(new NSString(titleString), new NSString("title"));

            if (!string.IsNullOrWhiteSpace(bodyString))
                userInfo.SetValueForKey(new NSString(bodyString), new NSString("body"));

            if (!string.IsNullOrWhiteSpace(urlString))
            {
                // store url in payload
                userInfo.SetValueForKey(new NSString(urlString), new NSString("url"));

                // 🔹 this is the key line:
                // only when URL exists → assign category → iOS shows "Open in Browser" button
                content.CategoryIdentifier = "OPEN_URL_CATEGORY";
            }

            content.UserInfo = userInfo;

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.5, false);
            var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);
            
            UNUserNotificationCenter.Current.AddNotificationRequest(request, _ => { });
        }
        
        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Debug.WriteLine(error.Description);
        }
        
        [Export("application:didFinishLaunchingWithOptions:")]
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            if (!DeviceInstallationService.NotificationsSupported) return base.FinishedLaunching(app, options);
    
            var center = UNUserNotificationCenter.Current;
    
            var openAction = UNNotificationAction.FromIdentifier(
                "OPEN_IN_BROWSER",
                "Open in Browser",
                UNNotificationActionOptions.Foreground);

            var openUrlCategory = UNNotificationCategory.FromIdentifier(
                "OPEN_URL_CATEGORY",
                new[] { openAction },
                Array.Empty<string>(),
                UNNotificationCategoryOptions.None);

            center.SetNotificationCategories(new NSSet<UNNotificationCategory>(openUrlCategory));

            // 3. IMPORTANT: set delegate BEFORE finishing launch
            UNUserNotificationCenter.Current.Delegate = new NotificationCenterDelegate();

            return base.FinishedLaunching(app, options);
        }
    }