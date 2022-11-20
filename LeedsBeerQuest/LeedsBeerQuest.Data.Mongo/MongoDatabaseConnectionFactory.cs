using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Data.Mongo
{
    public class MongoDatabaseConnectionFactory : IMongoDatabaseConnectionFactory
    {
        private readonly MongoDbSettings _settings;

        public MongoDatabaseConnectionFactory(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
        }

        public IMongoDatabase ConnectToDatabase()
        {
            var settings = MongoClientSettings.FromConnectionString(_settings.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            return client.GetDatabase("LeedsBeerQuest"); //TODO : Name of DB should be in config
        }
    }
}