using System.Text;
using System.Text.Json;
using GenericRestHelper.Interfaces;
using Microsoft.Extensions.Logging;
namespace GenericRestHelper.Services
{
    public class RestClientService : IRestClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestClientService> _logger;
        private readonly JsonSerializerOptions _options;

        public RestClientService(HttpClient httpClient, ILogger<RestClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<TResponse?> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null)
            => await SendRequestAsync<object, TResponse>(url, HttpMethod.Get, null, headers);

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null)
            => await SendRequestAsync<TRequest, TResponse>(url, HttpMethod.Post, data, headers);
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null)
           => await SendRequestAsync<TRequest, TResponse>(url, HttpMethod.Put, data, headers);

        public async Task<bool> DeleteAsync(string url, Dictionary<string, string>? headers = null)
        {
            var response = await SendRequestAsync<object, object>(url, HttpMethod.Delete, null, headers);
            return true; // تعتمد على منطق الـ API الخاص بك
        }
        private async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(string url, HttpMethod method, TRequest? data, Dictionary<string, string>? headers)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                if (headers != null)
                    foreach (var header in headers) request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                // When we post data
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data, _options);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API Error: {StatusCode} - {Content}", response.StatusCode, error);
                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return default;
                }
                return JsonSerializer.Deserialize<TResponse>(content, _options);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception during API call to {Url}", url);
                throw;
            }
        }

       
    }
}