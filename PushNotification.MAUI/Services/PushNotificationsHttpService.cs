using Microsoft.Extensions.Options;
using PushNotification.MAUI.Configurations;
using PushNotification.MAUI.Options;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Services;

public class PushNotificationsHttpService : IPushNotificationsHttpService
{
     private readonly IHttpService _httpService;
    private readonly IOptions<PushNotificationOptions> _pushNotificationOptions;

    public PushNotificationsHttpService(IHttpService httpService,
        IOptions<PushNotificationOptions> pushNotificationOptions)
    {
        _httpService = httpService;
        _pushNotificationOptions = pushNotificationOptions;
    }

    public Task<TResult?> PostAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class
    {
        return _httpService.PostAsync<TResult, TRequest>(uriPrefix, body, ApiType.EpicAppApi,
            new Dictionary<string, string>
            {
                { nameof(_pushNotificationOptions.Value.ApiKey), _pushNotificationOptions.Value.ApiKey }
            }, null);
    }

    public Task<TResult?> DeleteAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class
    {
        return _httpService.DeleteAsync<TResult, TRequest>(uriPrefix, body, ApiType.EpicAppApi,
            new Dictionary<string, string>
            {
                { nameof(_pushNotificationOptions.Value.ApiKey), _pushNotificationOptions.Value.ApiKey }
            }, null);
    }

    public Task<TResult?> GetAsync<TResult>(string uriPrefix, bool handleAuthError = true)
        where TResult : class, new()
    {
        return _httpService.GetAsync<TResult>(uriPrefix, ApiType.EpicAppApi,
            new Dictionary<string, string>
            {
                { nameof(_pushNotificationOptions.Value.ApiKey), _pushNotificationOptions.Value.ApiKey }
            }, null);
    }

    public Task<TResult?> PutAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class
    {
        return _httpService.PutAsync<TResult, TRequest>(uriPrefix, body, ApiType.EpicAppApi,
            new Dictionary<string, string>
            {
                { nameof(_pushNotificationOptions.Value.ApiKey), _pushNotificationOptions.Value.ApiKey }
            }, null);
    }
}
