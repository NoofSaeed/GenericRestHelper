using GenericRestHelper.Interfaces;
using GenericRestHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRestHelper.Extensions
{
    
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddGenericRestClient(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            // تسجيل الخدمة الأساسية
            services.AddScoped<IRestClientService, RestClientService>();

            // تسجيل الـ HttpClient وإرجاع الـ Builder
            return services.AddHttpClient<IRestClientService, RestClientService>(configureClient);
        }
    }
}
