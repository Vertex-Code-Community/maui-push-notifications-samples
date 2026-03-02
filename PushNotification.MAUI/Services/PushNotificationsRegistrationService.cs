using PushNotification.MAUI.Models.PushNotification;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Services;

public class PushNotificationsRegistrationService : IPushNotificationsRegistrationService
{
    private readonly IDeviceInstallationService _deviceInstallationService;
    private readonly IPushNotificationsHttpService _pushNotificationsHttpService;

    public PushNotificationsRegistrationService(IPushNotificationsHttpService pushNotificationsHttpService,
        IDeviceInstallationService deviceInstallationService)
    {
        _deviceInstallationService = deviceInstallationService;
        _pushNotificationsHttpService = pushNotificationsHttpService;
        
    }

    public Task DeregisterDeviceAsync()
    {
        return Task.CompletedTask;
    }

    public async Task RegisterDeviceAsync()
    {
        var tags = await GetLatestTagsAsync();
        var deviceInstallation = await _deviceInstallationService.GetDeviceInstallationAsync(tags);
        if (deviceInstallation is null) return;

        await _pushNotificationsHttpService
            .PutAsync<DeviceInstallation, DeviceInstallation>(
                $"api/Notifications/installations", deviceInstallation)
            .ConfigureAwait(false);
    }

    private async Task<List<string>> GetLatestTagsAsync()
    {
        bool isActualVersion = false;
       
        //here you can add yours tags, for example, you can check the version of the app and add a tag with the version, or add a tag with the user id, etc.
        var tags = new List<string>
        {
            $"version:{(isActualVersion ? "latest" : "old")}",
        }; 

        return tags;
    }

}