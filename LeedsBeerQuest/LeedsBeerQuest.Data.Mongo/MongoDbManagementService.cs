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
    public class MongoDbDataManagementService : MongoDbBase, IDataManagementService
    {
        public MongoDbDataManagementService(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

        public void ImportData(BeerEstablishment[] establishments)
        {
            IMongoDatabase database = ConnectToDatabase();

            var collection = database.GetCollection<BeerEstablishment>("Venues");
            collection.DeleteMany(Builders<BeerEstablishment>.Filter.Empty);
            collection.InsertMany(establishments);
        }
    }
}
