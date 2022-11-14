using LeedsBeerQuest.Api.Services;
using LeedsBeerQuest.Api.Settings;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace LeedsBeerQuest.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<DataImporter>();
            services.AddScoped<IBeerEstablishmentDataParser, BeerEstablishmentDataParser>();
            services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            services.AddScoped<IDataManagementService, InMemoryDataManagementService>();
            services.AddScoped<IFindMeBeerService, FindMeBeerService>();
            services.AddScoped<ILocationDistanceCalculator, LocationDistanceCalculator>();
            services.AddHttpClient<DataImporter>(client =>
            {
                var beerQuestApiEndpoint = config.GetValue<Uri>("BeerQuestData:ApiEndpointAddress");
                client.BaseAddress = beerQuestApiEndpoint;
            });
            return services;
        }

        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FindMeBeerServiceSettings>(config.GetSection("FindMeBeerServiceSettings"));
            return services;
        }
    }

}
