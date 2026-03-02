using Android.Provider;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Services;

public class DeviceInfoAndroidService : IDeviceInfoService
{
    
    public async Task<string> GetDeviceId()
    {
        return Settings.Secure.GetString(Platform.AppContext.ContentResolver, Settings.Secure.AndroidId) ?? string.Empty;
    }

    public async Task<string> GetSystemVersion()
    {
        return DeviceInfo.Current.VersionString;
    }
}