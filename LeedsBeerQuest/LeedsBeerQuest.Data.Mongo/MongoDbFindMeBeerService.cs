using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.App.Services;
using LeedsBeerQuest.App.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Data.Mongo
{
    public class MongoDbFindMeBeerService : IFindMeBeerService
    {
        private readonly IMongoDatabaseConnectionFactory _connFactory;
        private readonly IMongoQueryBuilder _queryBuilder;
        private readonly FindMeBeerServiceSettings _settings;

        public MongoDbFindMeBeerService(IMongoDatabaseConnectionFactory connFactory, IMongoQueryBuilder queryBuilder, IOptions<FindMeBeerServiceSettings> settings)
        {
            _connFactory = connFactory;
            _queryBuilder = queryBuilder;
            _settings = settings.Value;
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation)
        {
            var database = _connFactory.ConnectToDatabase();
            var collection = database.GetCollection<BeerEstablishment>("Venues");

            var startLocation = myLocation ?? _settings.DefaultSearchLocation;
            var pipeline = _queryBuilder
                .CreateGeoNearDocument(startLocation.Long, startLocation.Lat, "Location.Coordinates", "DistanceInMetres")
                .CreateProjectionStage(new[] { "Name", "Location", "DistanceInMetres" }, true)
                .CreateLimitStage(_settings.DefaultPageSize)
                .BuildPipeline();
            IAsyncCursor<BeerEstablishmentLocation> cursor = collection.Aggregate(pipeline);
            return (await cursor.ToListAsync()).ToArray();
        }
    }
}
