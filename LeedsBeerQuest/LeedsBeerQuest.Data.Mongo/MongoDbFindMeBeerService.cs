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
        public MongoDbFindMeBeerService(IOptions<MongoDbSettings> settings)
        {
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation)
        {
            //var database = ConnectToDatabase();
            //var collection = database.GetCollection<BeerEstablishment>("Venues");

            //PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline = new BsonDocument[]
            //{
            //    CreateGeoNearStage(-1.555570961376415, 53.801196991070164),
            //    CreateProjectionStage(),
            //    CreateLimitStage(10)
            //};

            //var cursor = collection.Aggregate(pipeline);
            //return (await cursor.ToListAsync()).ToArray();

            return Array.Empty<BeerEstablishmentLocation>();
        }

        private BsonDocument CreateGeoNearStage(double lng, double lat)
        {
            return new BsonDocument("$geoNear",
                new BsonDocument
                {
                    { "near",
                        new BsonDocument
                        {
                            { "type", "Point" },
                            { "coordinates",
                            new BsonArray
                            {
                                lng,
                                lat
                            }}
                    }    },
                    { "key", "Location.Coordinates" },
                    { "distanceField", "Distance" },
                    { "query", GetQuery() }
                });
        }

        private BsonDocument GetQuery()
        {
            return new BsonDocument("Category", new BsonDocument("$ne", "Closed venues"));
        }

        private BsonDocument CreateProjectionStage()
        {
            return new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 0 },
                { "Name", 1 },
                { "Distance", 1 },
                { "Location", 1 }
            });
        }

        private BsonDocument CreateLimitStage(int pageSize)
        {
            return new BsonDocument("$limit", pageSize);
        }

        
    }
}
