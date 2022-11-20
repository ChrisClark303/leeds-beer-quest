using LeedsBeerQuest.App.Models.Read;
using MongoDB.Bson;
using MongoDB.Driver;

public interface IMongoQueryBuilder
{
    IMongoQueryBuilder CreateGeoNearDocument(double lng, double lat, string keyName, string distanceFieldName);
    IMongoQueryBuilder CreateLimitStage(int pageSize);
    IMongoQueryBuilder CreateNotEqualQuery(string fieldName, string fieldValue);
    IMongoQueryBuilder CreateProjectionStage(string[] fieldsToInclude, bool excludeId = false);
    BsonDocument[] Build();
    PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation> BuildPipeline();
}