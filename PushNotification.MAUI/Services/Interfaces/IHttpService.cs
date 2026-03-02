namespace PushNotification.MAUI.Services.Interfaces;

public interface IHttpService
{
    public Task<TResult?> PostAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null, 
        Func<Task>? unauthorizedAction = null)
        where TResult : class, new()
        where TRequest : class;
    
    public Task<TResult?> DeleteAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null,
        Func<Task>? unauthorizedAction = null)
        where TResult : class, new()
        where TRequest : class;

    Task<TResult?> GetAsync<TResult>(string uriPrefix, string apiType, IDictionary<string, string>? headers = null,
        Func<Task>? unauthorizedAction = null)
        where TResult : class, new();
    
    public Task<TResult?> PutAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null,
        Func<Task>? unauthorizedAction = null)
        where TResult : class, new()
        where TRequest : class;
}