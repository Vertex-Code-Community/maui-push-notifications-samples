using Microsoft.Extensions.Options;
using PushNotification.MAUI.Options;
using PushNotification.MAUI.Services;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Configurations;

public static class DependencyStartup
{
    public static void ConfigureDependencies(this MauiAppBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        builder.Services.AddHttpClient();
        
        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<IPushNotificationPermissionService, PushNotificationPermissionService>();
        services.AddSingleton<IPushNotificationsRegistrationService, PushNotificationsRegistrationService>();
        services.AddSingleton<IPushNotificationsHttpService, PushNotificationsHttpService>();
        services.AddSingleton<IPushNotificationsMessageService, PushNotificationsMessageService>();

        services.Configure<PushNotificationOptions>(configuration.GetSection(nameof(PushNotificationOptions)));

        builder.Services.AddHttpClient(ApiType.EpicAppApi, (sp, client) =>
            {
                client.BaseAddress = new Uri("https://unsoothed-stemless-jeanne.ngrok-free.dev/");
            })

#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
#endif
            ;
        
        builder.ConfigureAndroidDependencies();
        builder.ConfigureIosDependencies();
    }


    private static void ConfigureIosDependencies(this MauiAppBuilder builder)
    {
        var services = builder.Services;

#if IOS
        services.AddSingleton<IDeviceInstallationService, DeviceInstallationIOSService>();
        services.AddSingleton<IDeviceInfoService, DeviceInfoIOSService>();
#endif
    }

    private static void ConfigureAndroidDependencies(this MauiAppBuilder builder)
    {
        var services = builder.Services;
#if ANDROID
        services.AddSingleton<IDeviceInstallationService, DeviceInstallationAndroidService>();
        services.AddSingleton<IDeviceInfoService, DeviceInfoAndroidService>();
#endif
    }
}