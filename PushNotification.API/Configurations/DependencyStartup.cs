using PushNotification.API.Services;
using PushNotification.API.Services.Interfaces;

namespace PushNotification.API.Configurations;

public static class DependencyStartup
{
    public static void ConfigurationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationHubService, NotificationHubService>();
        services.AddScoped<INotificationService, NotificationService>();
    }
}