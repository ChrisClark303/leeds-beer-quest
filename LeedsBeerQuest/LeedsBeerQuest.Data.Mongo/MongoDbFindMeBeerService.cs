using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using LeedsBeerQuest.App.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

        public async Task<BeerEstablishment?> GetBeerEstablishmentByName(string establishmentName)
        {
            var filter = Builders<BeerEstablishment>.Filter.Eq(f => f.Name, establishmentName);
            var projection = Builders<BeerEstablishment>.Projection
                .Exclude("Location._t")
                .Exclude("Location.Coordinates")
                .Exclude("_id");
            var options = new FindOptions<BeerEstablishment> { Projection = projection };

            var collection = GetVenuesCollection();
            var resultsCursor = await collection.FindAsync(filter, options);
            return await resultsCursor.FirstOrDefaultAsync();
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null)
        {
            var searchLocation = myLocation ?? _settings.DefaultSearchLocation!;
            var pipeline = _queryBuilder
                .WithAggregationGeoNear(searchLocation.Long, searchLocation.Lat, "Location.Coordinates", "DistanceInMetres")
                .WithAggregationProjection(new[] { "Name", "Location.Lat", "Location.Long", "DistanceInMetres" }, excludeId: true)
                .WithAggregationLimit(_settings.DefaultPageSize)
                .BuildPipeline();

            var collection = GetVenuesCollection();
            var resultsCursor = collection.Aggregate(pipeline);
            return (await resultsCursor.ToListAsync()).ToArray();
        }

        private IMongoCollection<BeerEstablishment> GetVenuesCollection()
        {
            var database = _connFactory.ConnectToDatabase();
            return database.GetCollection<BeerEstablishment>("Venues");
        }
    }
}
