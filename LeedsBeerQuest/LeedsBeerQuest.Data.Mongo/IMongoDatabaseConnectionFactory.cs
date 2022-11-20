using MongoDB.Driver;

namespace LeedsBeerQuest.Data.Mongo
{
    public interface IMongoDatabaseConnectionFactory
    {
        IMongoDatabase ConnectToDatabase();
    }
}