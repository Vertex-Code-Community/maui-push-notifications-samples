using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PushNotification.MAUI.Services.Interfaces;

namespace PushNotification.MAUI.Services;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public HttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
 public Task<TResult?> DeleteAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null, Func<Task>? unauthorizedAction = null) where TResult : class, new() where TRequest : class
    {
        Console.WriteLine($"REQUEST: {uriPrefix}");
        
        var request = new HttpRequestMessage(HttpMethod.Delete, uriPrefix);

        if (body is not null)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(body));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        
        return SendAsync<TResult>(request, apiType, headers ?? new Dictionary<string, string>(), unauthorizedAction);
    }

    public Task<TResult?> GetAsync<TResult>(string uriPrefix, string apiType, IDictionary<string, string>? headers = null, Func<Task>? unauthorizedAction = null) 
        where TResult : class, new()
    {
        Console.WriteLine($"REQUEST: {uriPrefix}");

        var request = new HttpRequestMessage(HttpMethod.Get, uriPrefix);
        return SendAsync<TResult>(request, apiType, headers ?? new Dictionary<string, string>(), unauthorizedAction);
    }

    public Task<TResult?> PutAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null, Func<Task>? unauthorizedAction = null) where TResult : class, new() where TRequest : class
    {
        Console.WriteLine($"REQUEST: {uriPrefix}");
        
        var request = new HttpRequestMessage(HttpMethod.Put, uriPrefix);

        if (body is not null)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(body));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        
        return SendAsync<TResult>(request, apiType, headers ?? new Dictionary<string, string>(), unauthorizedAction);
    }

    public Task<TResult?> PostAsync<TResult, TRequest>(string uriPrefix, TRequest? body, string apiType, IDictionary<string, string>? headers = null, Func<Task>? unauthorizedAction = null) 
        where TResult : class, new()
        where TRequest : class
    {
        Console.WriteLine($"REQUEST: {uriPrefix}");
        
        var request = new HttpRequestMessage(HttpMethod.Post, uriPrefix);

        if (body is not null)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(body));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        
        return SendAsync<TResult>(request, apiType, headers ?? new Dictionary<string, string>(), unauthorizedAction);
    }
    
    private async Task<TResult?> SendAsync<TResult>(HttpRequestMessage request, string apiType, IDictionary<string, string> headers, Func<Task>? unauthorizedAction)
        where TResult : class, new()
    {
        var exIdentifier = Guid.NewGuid().ToString();
        
        try
        {
            Console.WriteLine($"BASE URL 0: {apiType} {exIdentifier}");
            var httpClient = _httpClientFactory.CreateClient(apiType);
            Console.WriteLine($"BASE URL: {httpClient.BaseAddress}");
            
            foreach (var keyVal in headers)
                request.Headers.Add(keyVal.Key, keyVal.Value);
            
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                await (unauthorizedAction?.Invoke() ?? Task.CompletedTask);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"_______ERROR________ {errorContent}");    
            }
            
            var content = await response.Content.ReadAsStringAsync();
            if (typeof(TResult) == typeof(string)) return content as TResult;

            try
            {
                return JsonConvert.DeserializeObject<TResult>(content) ?? new();
            }
            catch
            {
                return new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"______________________________ {exIdentifier}");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace ?? string.Empty);
            
            return new();
        }
    }
}