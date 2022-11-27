using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Data.Mongo
{
    public class MongoDbManagementServiceTests
    {
        [Test]
        public async Task ImportData_RetrievesConnectionToDatabase_FromDatabaseConnectionFactory()
        {
            var dbConnFactory = new Mock<IMongoDatabaseConnectionFactory>();

            var mgmtSvc = CreateManagementService(dbConnFactory);
            await mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            dbConnFactory.Verify(f => f.ConnectToDatabase());
        }

        [Test]
        public async Task ImportData_RetrievesVenuesCollection_FromDatabase()
        {
            var database = new Mock<IMongoDatabase>();

            var mgmtSvc = CreateManagementService(database: database);
            await mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            database.Verify(f => f.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()));
        }

        [Test]
        public async Task ImportData_ClearsDownExistingData_FromCollection()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(collection: collection);
            await mgmtSvc.ImportData(Array.Empty<BeerEstablishment>());

            collection.Verify(c => c.DeleteManyAsync(Builders<BeerEstablishment>.Filter.Empty, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ImportData_InsertsBeerEstablisments_ToCollection()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(collection: collection);
            var establishments = new[] { new BeerEstablishment() };
            await mgmtSvc.ImportData(establishments);

            collection.Verify(c => c.InsertManyAsync(establishments, It.IsAny<InsertManyOptions>(),CancellationToken.None));
        }

        [Test]
        public async Task ImportData_CreatesLocationIndex()
        {
            var idxMgr = new Mock<IMongoIndexManager<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(indexManager: idxMgr);
            var establishments = new[] { new BeerEstablishment() };
            await mgmtSvc.ImportData(establishments);

            //Couldn't figure out a way of inspecting the internals of CreateIndexModel to check the index was being created with the correct properties
            idxMgr.Verify(i => i.CreateOneAsync(It.IsAny<CreateIndexModel<BeerEstablishment>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()));
        }

        private MongoDbDataManagementService CreateManagementService(Mock<IMongoDatabaseConnectionFactory>? factory = null,
            Mock<IMongoDatabase>? database = null,
            Mock<IMongoCollection<BeerEstablishment>>? collection = null,
            Mock<IMongoIndexManager<BeerEstablishment>>? indexManager = null)
        {
            var db = database ?? new Mock<IMongoDatabase>();
            var dbConnFactory = factory ?? new Mock<IMongoDatabaseConnectionFactory>();
            var coll = collection ?? new Mock<IMongoCollection<BeerEstablishment>>();
            var idxMgr = indexManager ?? new Mock<IMongoIndexManager<BeerEstablishment>>();
            dbConnFactory.Setup(f => f.ConnectToDatabase())
                .Returns(db.Object);
            db.Setup(d => d.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);
            coll.Setup(c => c.Indexes)
                .Returns(idxMgr.Object);

            return new MongoDbDataManagementService(dbConnFactory.Object);
        }
    }
}
