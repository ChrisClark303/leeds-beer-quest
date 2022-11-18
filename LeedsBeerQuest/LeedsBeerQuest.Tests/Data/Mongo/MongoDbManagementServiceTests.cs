using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Data.Mongo
{
    public class MongoDbManagementServiceTests
    {
        private MongoDbDataManagementService CreateManagementService(Mock<IMongoDatabaseConnectionFactory>? factory = null, Mock<IMongoDatabase>? database = null, Mock<IMongoCollection<BeerEstablishment>>? collection = null)
        {
            var db = database ?? new Mock<IMongoDatabase>();
            var dbConnFactory = factory ?? new Mock<IMongoDatabaseConnectionFactory>();
            dbConnFactory.Setup(f => f.ConnectToDatabase())
                .Returns(db.Object);
            db.Setup(d => d.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()))
                .Returns((collection ?? new Mock<IMongoCollection<BeerEstablishment>>()).Object);

            return new MongoDbDataManagementService(dbConnFactory.Object);
        }


        [Test]
        public void ImportData_RetrievesConnectionToDatabase_FromDatabaseConnectionFactory()
        {
            var dbConnFactory = new Mock<IMongoDatabaseConnectionFactory>();

            var mgmtSvc = CreateManagementService(dbConnFactory);
            mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            dbConnFactory.Verify(f => f.ConnectToDatabase());
        }

        [Test]
        public void ImportData_RetrievesVenuesCollection_FromDatabase()
        {
            var database = new Mock<IMongoDatabase>();

            var mgmtSvc = CreateManagementService(database: database);
            mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            database.Verify(f => f.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()));
        }

        [Test]
        public void ImportData_ClearsDownExistingData_FromCollection()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(collection: collection);
            mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            collection.Verify(c => c.DeleteMany(Builders<BeerEstablishment>.Filter.Empty, It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ImportData_InsertsBeerEstablisments_ToCollection()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(collection: collection);
            var establishments = new[] { new BeerEstablishment() };
            mgmtSvc.ImportData(establishments);

            collection.Verify(c => c.InsertMany(establishments, It.IsAny<InsertManyOptions>(),CancellationToken.None));
        }

        [Test]
        public void ImportData_CreatesLocationIndex()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();
            var idxMgr = new Mock<IMongoIndexManager<BeerEstablishment>>();
            collection.Setup(c => c.Indexes)
                .Returns(idxMgr.Object);

            var mgmtSvc = CreateManagementService(collection: collection);
            var establishments = new[] { new BeerEstablishment() };
            mgmtSvc.ImportData(establishments);

            idxMgr.Verify(i => i.CreateOne(It.Is<CreateIndexModel<BeerEstablishment>>(m => CheckCreateIndexModel(m)), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()));
        }

        private bool CheckCreateIndexModel(CreateIndexModel<BeerEstablishment> model)
        {
            var x = model.Keys.ToBsonDocument();
            var json = x.ToJson();
            var s = model.ToString();
            return s == "{ location: \"2dsphere\" }";
        }
    }
}
