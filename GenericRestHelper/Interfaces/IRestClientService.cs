namespace GenericRestHelper.Interfaces
{
    public interface IRestClientService
    {
        Task<TResponse?> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null);
        Task<bool> DeleteAsync(string url, Dictionary<string, string>? headers = null);
        Task<TResponse?> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent content, Dictionary<string, string>? headers = null);
        Task<TResponse?> PutMultipartAsync<TResponse>(string url, MultipartFormDataContent content, Dictionary<string, string>? headers = null);
    }
}
