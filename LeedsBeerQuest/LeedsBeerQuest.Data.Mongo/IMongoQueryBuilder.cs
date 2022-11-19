using LeedsBeerQuest.App.Models;
using MongoDB.Bson;
using MongoDB.Driver;

public interface IMongoQueryBuilder
{
    PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline();
    BsonDocument CreateGeoNearDocument(double lng, double lat, string keyName, string distanceFieldName);
    BsonDocument CreateLimitStage(int pageSize);
    BsonDocument CreateNotEqualQuery(string fieldName, string fieldValue);
    BsonDocument CreateProjectionStage(string[] fieldsToInclude, bool excludeId = false);
}