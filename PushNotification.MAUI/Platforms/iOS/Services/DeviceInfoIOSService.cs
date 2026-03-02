using PushNotification.MAUI.Services.Interfaces;
using UIKit;

namespace PushNotification.Maui.Services;

public class DeviceInfoIOSService : IDeviceInfoService
{
    
    public async Task<string> GetDeviceId()
    {
        return UIDevice.CurrentDevice.IdentifierForVendor.ToString() ?? string.Empty;
    }

    public async Task<string> GetSystemVersion()
    {
        return DeviceInfo.Current.VersionString;
    }
}