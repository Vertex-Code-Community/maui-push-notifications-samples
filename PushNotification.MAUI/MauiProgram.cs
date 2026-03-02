using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PushNotification.MAUI.Configurations;

namespace PushNotification.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
            .GetAwaiter().GetResult();

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));

        builder.Services.AddMauiBlazorWebView();
        
        builder.ConfigureDependencies();
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}