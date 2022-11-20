using LeedsBeerQuest.App.Models;
using MongoDB.Bson;
using MongoDB.Driver;

//TODO : Extract operation names to consts
public class MongoQueryBuilder : IMongoQueryBuilder
{
    private List<BsonDocument> _documents = new List<BsonDocument>();

    public IMongoQueryBuilder CreateGeoNearDocument(double lng, double lat, string keyName, string distanceFieldName)
    {
        var doc = new BsonDocument("$geoNear",
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
        _documents.Add(doc);
        return this;
    }

    public IMongoQueryBuilder CreateNotEqualQuery(string fieldName, string fieldValue)
    {
        var doc = new BsonDocument(fieldName, new BsonDocument("$ne", fieldValue));
        _documents.Add(doc);

        return this;
    }

    public IMongoQueryBuilder CreateProjectionStage(string[] fieldsToInclude, bool excludeId = false)
    {
        var projectedFieldDict = new Dictionary<string, int>(fieldsToInclude.Select(f => new KeyValuePair<string, int>(f, 1)));
        if (excludeId)
        {
            projectedFieldDict.Add("_id", 0);
        };

        var doc = new BsonDocument("$project",
            new BsonDocument(projectedFieldDict)
        );
        _documents.Add(doc);
        return this;
    }

    public IMongoQueryBuilder CreateLimitStage(int pageSize)
    {
        var doc = new BsonDocument("$limit", pageSize);
        _documents.Add(doc);
        return this;
    }

    public PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline()
    {
        PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline = Build();
        return pipeline;
    }

    public BsonDocument[] Build()
    {
        return _documents.ToArray();
    }
}