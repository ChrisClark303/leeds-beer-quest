using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Data.Mongo
{
    public class MongoDbDataManagementService : IDataManagementService
    {
        private readonly IMongoDatabaseConnectionFactory _connectionFactory;

        public MongoDbDataManagementService(IMongoDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void ImportData(BeerEstablishment[] establishments)
        {
            IMongoDatabase database = _connectionFactory.ConnectToDatabase();

            var collection = database.GetCollection<BeerEstablishment>("Venues");
            collection.DeleteMany(Builders<BeerEstablishment>.Filter.Empty);
            collection.InsertMany(establishments);
            var indexKey = Builders<BeerEstablishment>.IndexKeys.Geo2DSphere(v => v.Location);
            collection.Indexes.CreateOne(new CreateIndexModel<BeerEstablishment>(indexKey));
        }
    }
}
