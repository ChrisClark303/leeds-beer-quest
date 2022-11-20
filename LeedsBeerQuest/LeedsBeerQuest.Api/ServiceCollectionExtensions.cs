using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Services;
using LeedsBeerQuest.App.Settings;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace LeedsBeerQuest.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            var useInMemoryData = config.GetValue<bool>("UseInMemoryData");
            if (useInMemoryData)
            {
                services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
                services.AddScoped<IFindMeBeerService, FindMeBeerService>();
                services.AddScoped<IDataManagementService, InMemoryDataManagementService>();
            }
            else
            {
                services.AddScoped<IMongoDatabaseConnectionFactory, MongoDatabaseConnectionFactory>();
                services.AddScoped<IDataManagementService, MongoDbDataManagementService>();
                services.AddScoped<IFindMeBeerService, MongoDbFindMeBeerService>();
                services.AddScoped<IMongoQueryBuilder, MongoQueryBuilder>();
            }

            services.AddScoped<DataImporter>();
            services.AddScoped<IBeerEstablishmentDataParser, BeerEstablishmentDataParser>();
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
            services
                .Configure<FindMeBeerServiceSettings>(config.GetSection("FindMeBeerServiceSettings"))
                .Configure<MongoDbSettings>(config.GetSection("MongoDBSettings"));
            return services;
        }
    }

}
