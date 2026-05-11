using GenericRestHelper.Interfaces;
using GenericRestHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRestHelper.Extensions
{
    
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericRestClient(this IServiceCollection services)
        {
            services.AddHttpClient<IRestClientService, RestClientService>();
            return services;
        }
    }
}
