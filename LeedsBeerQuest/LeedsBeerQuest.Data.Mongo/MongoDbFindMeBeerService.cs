using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.App.Services;
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

        public MongoDbFindMeBeerService(IMongoDatabaseConnectionFactory connFactory, IMongoQueryBuilder queryBuilder)
        {
            _connFactory = connFactory;
            _queryBuilder = queryBuilder;
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation)
        {
            var database = _connFactory.ConnectToDatabase();
            var collection = database.GetCollection<BeerEstablishment>("Venues");

            //PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline = new BsonDocument[]
            //{
            //    CreateGeoNearStage(-1.555570961376415, 53.801196991070164),
            //    CreateProjectionStage(),
            //    CreateLimitStage(10)
            //};

            var pipeline = _queryBuilder.BuildPipeline();
            IAsyncCursor<BeerEstablishmentLocation> cursor = collection.Aggregate(pipeline);
            return (await cursor.ToListAsync()).ToArray();

            //return Array.Empty<BeerEstablishmentLocation>();
        }


    }
}

public class MongoQueryBuilder : IMongoQueryBuilder
{
    public BsonDocument CreateGeoNearDocument(double lng, double lat, string keyName, string distanceFieldName)
    {
        return new BsonDocument("$geoNear",
            new BsonDocument
            {
                    { "near", new BsonDocument
                        {
                            { "type", "Point" },
                            { "coordinates",
                            new BsonArray
                            {
                                lng,
                                lat
                            }}
                    }    },
                    { "key", keyName },
                    { "distanceField", distanceFieldName },
                   // { "query", CreateNotEqualQuery("Category", "Closed venues") } //TODO : pass in query details
            });
    }

    public BsonDocument CreateNotEqualQuery(string fieldName, string fieldValue)
    {
        return new BsonDocument(fieldName, new BsonDocument("$ne", fieldValue));
    }

    public BsonDocument CreateProjectionStage(string[] fieldsToInclude, bool excludeId = false)
    {
        var projectedFieldDict = new Dictionary<string, int>(fieldsToInclude.Select(f => new KeyValuePair<string, int>(f, 1)));
        if (excludeId)
        {
            projectedFieldDict.Add("_id", 0);
        };

        return new BsonDocument("$project",
            new BsonDocument(projectedFieldDict)
        );
    }

    public BsonDocument CreateLimitStage(int pageSize)
    {
        return new BsonDocument("$limit", pageSize);
    }

    public PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline()
    {
        PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline = new BsonDocument[]
        {
            CreateGeoNearDocument(-1.555570961376415, 53.801196991070164, "Location.Coordinates", "DistanceInMetres"),
            //CreateProjectionStage(new [] { "Name" }, true),
            //CreateLimitStage(10)
        };

        return pipeline;
    }
}