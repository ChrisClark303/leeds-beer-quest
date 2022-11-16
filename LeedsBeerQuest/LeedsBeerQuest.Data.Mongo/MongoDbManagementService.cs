using LeedsBeerQuest.Data.Models;
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
        private readonly MongoDbSettings _settings;

        public MongoDbDataManagementService(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
        }

        public void ImportData(BeerEstablishment[] establishments)
        {
            IMongoDatabase database = ConnectToDatabase();

            var collection = database.GetCollection<BeerEstablishment>("Venues");
            collection.DeleteMany(Builders<BeerEstablishment>.Filter.Empty);
            collection.InsertMany(establishments);
        }

        private IMongoDatabase ConnectToDatabase()
        {
            var settings = MongoClientSettings.FromConnectionString(_settings.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            return client.GetDatabase("LeedsBeerQuest");
        }
    }
}
