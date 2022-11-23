using LeedsBeerQuest.App.Models.Read;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

public enum ProjectionType
{
    Exclude = 0,
    Include = 1
}

//TODO : Extract operation names to consts
public class MongoQueryBuilder : IMongoQueryBuilder
{
    private readonly List<BsonDocument> _documents = new List<BsonDocument>();

    public IMongoQueryBuilder WithAggregationGeoNear(double lng, double lat, string keyName, string distanceFieldName)
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
                   // { "query", WithNotEqualQuery("Category", "Closed venues") } //TODO : pass in query details
            });
        _documents.Add(doc);
        return this;
    }

    public IMongoQueryBuilder WithNotEqualQuery(string fieldName, string fieldValue)
    {
        var doc = new BsonDocument(fieldName, new BsonDocument("$ne", fieldValue));
        _documents.Add(doc);

        return this;
    }

    public IMongoQueryBuilder WithEqualQuery(string fieldName, string fieldValue)
    {
        var doc = new BsonDocument(fieldName, fieldValue);
        _documents.Add(doc);

        return this;
    }

    public IMongoQueryBuilder WithAggregationProjection(string[] fieldsToProject, ProjectionType projectionType, bool excludeId = false)
    {
        var doc = new BsonDocument("$project", CreateProjectionDocument(fieldsToProject, projectionType, excludeId));
        _documents.Add(doc);
        return this;
    }

    public IMongoQueryBuilder WithAggregationLimit(int pageSize)
    {
        var doc = new BsonDocument("$limit", pageSize);
        _documents.Add(doc);
        return this;
    }

    public IMongoQueryBuilder WithProjection(string[] fieldsToProject, ProjectionType projectionType, bool excludeId = false)
    {
        var document = CreateProjectionDocument(fieldsToProject, projectionType, excludeId);
        _documents.Add(document);
        return this;
    }

    private BsonDocument CreateProjectionDocument(string[] fieldsToProject, ProjectionType projectionType, bool excludeId = false)
    {
        var projectedFieldDict = new Dictionary<string, int>(fieldsToProject.Select(f => new KeyValuePair<string, int>(f, (int)projectionType)));
        if (excludeId)
        {
            projectedFieldDict.Add("_id", 0);
        }
        return new BsonDocument(projectedFieldDict);
    }

    public PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline()
    {
        //Ensure the implicit cast from BsonDocument[] to PipelineDefinition is triggered
        PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> pipeline = Build();
        return pipeline;
    }

    public BsonDocument[] Build()
    {
        //TODO : Reset the documents array at this point?
        return _documents.ToArray();
    }
}