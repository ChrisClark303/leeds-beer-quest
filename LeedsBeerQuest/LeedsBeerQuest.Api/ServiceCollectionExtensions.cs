using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Services;
using LeedsBeerQuest.App.Settings;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeedsBeerQuest.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            var useInMemoryData = config.GetValue<bool>("UseInMemoryData");
            if (useInMemoryData)
            {
                services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
                services.AddScoped<IFindMeBeerService, FindMeBeerService>();
                services.AddScoped<IDataManagementService, InMemoryDataManagementService>();
                services.AddScoped<ILocationDistanceCalculator, LocationDistanceCalculator>();
            }
            else
            {
                services.AddScoped<IMongoDatabaseConnectionFactory, MongoDatabaseConnectionFactory>();
                services.AddScoped<IDataManagementService, MongoDbDataManagementService>();
                services.AddScoped<IFindMeBeerService, MongoDbFindMeBeerService>();
                services.AddScoped<IMongoQueryBuilder, MongoQueryBuilder>();
            }

            services.AddScoped<IBeerEstablishmentDataParser, BeerEstablishmentDataParser>();
            services.AddScoped<DataImporter>();
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
