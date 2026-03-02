using Microsoft.AspNetCore.Components;
using PushNotification.MAUI.Services;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Components.Pages;

public partial class RegistrationPushNotification : ComponentBase
{
    [Inject] public required IPushNotificationsRegistrationService PushNotificationsRegistrationService { get; set; }
    [Inject] public required IPushNotificationsHttpService PushNotificationsHttpService { get; set; } 

    protected override void OnInitialized()
    {
        PushNotificationsRegistrationService.RegisterDeviceAsync();
    }
    
    private async  Task OnClickSendNotification()
    {
        await PushNotificationsHttpService
        .GetAsync<object>($"api/Notifications/send").ConfigureAwait(false);
    }
}