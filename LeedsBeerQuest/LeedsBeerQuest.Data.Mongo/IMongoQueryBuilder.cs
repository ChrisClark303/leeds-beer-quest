using LeedsBeerQuest.App.Models.Read;
using MongoDB.Bson;
using MongoDB.Driver;

public interface IMongoQueryBuilder
{
    IMongoQueryBuilder WithAggregationGeoNear(double lng, double lat, string keyName, string distanceFieldName);
    IMongoQueryBuilder WithAggregationLimit(int pageSize);
    IMongoQueryBuilder WithNotEqualQuery(string fieldName, string fieldValue);
    IMongoQueryBuilder WithAggregationProjection(string[] fieldsToProject, ProjectionType projectionType, bool excludeId = false);
    IMongoQueryBuilder WithProjection(string[] fieldsToProject, ProjectionType projectionType, bool excludeId = false);
    IMongoQueryBuilder WithIsEqualToQuery(string fieldName, string fieldValue);
    BsonDocument[] Build();
    PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline();
}