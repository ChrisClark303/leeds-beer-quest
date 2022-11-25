using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Models.Write;
using LocationWriteModel = LeedsBeerQuest.App.Models.Write.Location;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeedsBeerQuest.App.Services;

namespace LeedsBeerQuest.Data.Mongo
{
    public class MongoDbDataManagementService : IDataManagementService
    {
        private readonly IMongoDatabaseConnectionFactory _connectionFactory;

        public MongoDbDataManagementService(IMongoDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task ImportData(BeerEstablishment[] establishments)
        {
            IMongoDatabase database = _connectionFactory.ConnectToDatabase();
            var collection = database.GetCollection<BeerEstablishment>("Venues");
            
            await collection.DeleteManyAsync(Builders<BeerEstablishment>.Filter.Empty);
            await collection.InsertManyAsync(establishments);

            var indexKey = Builders<BeerEstablishment>.IndexKeys.Geo2DSphere("Location.Coordinates");
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<BeerEstablishment>(indexKey));
        }
    }
}
