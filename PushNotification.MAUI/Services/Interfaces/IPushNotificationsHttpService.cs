namespace PushNotification.MAUI.Services.Interfaces;

public interface IPushNotificationsHttpService
{
    public Task<TResult?> PostAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class;
    
    public Task<TResult?> DeleteAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class;

    Task<TResult?> GetAsync<TResult>(string uriPrefix, bool handleAuthError = true)
        where TResult : class, new();
    
    public Task<TResult?> PutAsync<TResult, TRequest>(string uriPrefix, TRequest? body, bool handleAuthError = true)
        where TResult : class, new()
        where TRequest : class;
}