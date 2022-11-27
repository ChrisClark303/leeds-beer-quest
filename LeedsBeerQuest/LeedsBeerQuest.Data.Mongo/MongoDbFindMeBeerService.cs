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
            var queryDocs = BuildEstablishmentByNameQuery(establishmentName);
            return await ExecuteQuery(queryDocs.filter, queryDocs.projection);
        }

        private (BsonDocument filter, BsonDocument projection) BuildEstablishmentByNameQuery(string establishmentName)
        {
            var documents = _queryBuilder
                .WithIsEqualToQuery("Name", establishmentName)
                .WithProjection(new[] { "Location._t", "Location.Coordinates", "_id" }, ProjectionType.Exclude)
                .Build();

            //Documents are built in the order they are specified, so should be accessed in the same order
            var filter = documents[0];
            var projection = documents[1];

            return (filter, projection);
        }

        private async Task<BeerEstablishment> ExecuteQuery(BsonDocument filter, BsonDocument projection)
        {
            var options = new FindOptions<BeerEstablishment> { Projection = projection };
            var collection = ConnectToVenuesCollection();
            var resultsCursor = await collection.FindAsync<BeerEstablishment>(filter, options);
            return await resultsCursor.FirstOrDefaultAsync();
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null)
        {
            var searchLocation = myLocation ?? _settings.DefaultSearchLocation!;
            var pipeline = BuildGetNearestLocationsPipeline(searchLocation);
            return await ExecuteAggregationPipeline(pipeline);
        }

        private PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildGetNearestLocationsPipeline(Location searchLocation)
        {
            return _queryBuilder
                .WithAggregationGeoNear(searchLocation.Long, searchLocation.Lat, "Location.Coordinates", "DistanceInMetres")
                .WithAggregationProjection(new[] { "Name", "Location.Lat", "Location.Long", "DistanceInMetres" }, ProjectionType.Include, excludeId: true)
                .WithAggregationLimit(_settings.DefaultPageSize)
                .BuildPipeline();
        }

        private async Task<BeerEstablishmentLocation[]> ExecuteAggregationPipeline(PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline)
        {
            var collection = ConnectToVenuesCollection();
            var resultsCursor = collection.Aggregate(pipeline);
            return (await resultsCursor.ToListAsync()).ToArray();
        }

        private IMongoCollection<BeerEstablishment> ConnectToVenuesCollection()
        {
            var database = _connFactory.ConnectToDatabase();
            return database.GetCollection<BeerEstablishment>("Venues");
        }
    }
}