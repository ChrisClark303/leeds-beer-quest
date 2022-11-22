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

        public async Task<BeerEstablishment> GetBeerEstablishmentByName(string establishmentName)
        {
            var database = _connFactory.ConnectToDatabase();
            var collection = database.GetCollection<BeerEstablishment>("Venues");

            var filter = Builders<BeerEstablishment>.Filter.Eq(f => f.Name, establishmentName);
            var projection = Builders<BeerEstablishment>.Projection
                .Exclude("Location._t")
                .Exclude("Location.Coordinates")
                .Exclude("_id");
            var options = new FindOptions<BeerEstablishment> { Projection = projection };
            var resultsCursor = await collection.FindAsync(filter, options);
            return await resultsCursor.FirstOrDefaultAsync();
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null)
        {
            var database = _connFactory.ConnectToDatabase();
            var collection = database.GetCollection<BeerEstablishment>("Venues");

            var startLocation = myLocation ?? _settings.DefaultSearchLocation!;
            var pipeline = _queryBuilder
                .CreateGeoNearDocument(startLocation.Long, startLocation.Lat, "Location.Coordinates", "DistanceInMetres")
                .CreateProjectionStage(new[] { "Name", "Location.Lat", "Location.Long", "DistanceInMetres" }, true)
                .CreateLimitStage(_settings.DefaultPageSize)
                .BuildPipeline();
            var resultsCursor = collection.Aggregate(pipeline);
            return (await resultsCursor.ToListAsync()).ToArray();
        }
    }
}
