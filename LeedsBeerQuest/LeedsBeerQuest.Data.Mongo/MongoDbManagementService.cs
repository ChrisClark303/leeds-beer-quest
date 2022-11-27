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
            var collection = ConnectToVenuesCollection();
            await RefreshEstablishmentData(collection, establishments);
            await CreateGeospatialIndex(collection);
        }

        private IMongoCollection<BeerEstablishment> ConnectToVenuesCollection()
        {
            var database = _connectionFactory.ConnectToDatabase();
            return database.GetCollection<BeerEstablishment>("Venues");
        }

        private static async Task RefreshEstablishmentData(IMongoCollection<BeerEstablishment> collection, BeerEstablishment[] establishments)
        {
            await collection.DeleteManyAsync(Builders<BeerEstablishment>.Filter.Empty);
            await collection.InsertManyAsync(establishments);
        }

        private static async Task CreateGeospatialIndex(IMongoCollection<BeerEstablishment> collection)
        {
            var indexKey = Builders<BeerEstablishment>.IndexKeys.Geo2DSphere("Location.Coordinates");
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<BeerEstablishment>(indexKey));
        }
    }
}
