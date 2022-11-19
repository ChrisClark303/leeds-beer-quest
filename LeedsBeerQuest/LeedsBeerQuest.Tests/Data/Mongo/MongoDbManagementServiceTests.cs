using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
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
            var idxMgr = new Mock<IMongoIndexManager<BeerEstablishment>>();

            var mgmtSvc = CreateManagementService(indexManager: idxMgr);
            var establishments = new[] { new BeerEstablishment() };
            mgmtSvc.ImportData(establishments);

            //Couldn't figure out a way of inspecting the internals of CreateIndexModel to check the index was being created with the correct properties
            idxMgr.Verify(i => i.CreateOne(It.IsAny<CreateIndexModel<BeerEstablishment>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()));
        }
    }

    public class MongoDbFindMeBeerServiceTests
    {
        private MongoDbFindMeBeerService CreateBeerService(Mock<IMongoDatabaseConnectionFactory>? factory = null,
                Mock<IMongoDatabase>? database = null,
                Mock<IMongoCollection<BeerEstablishment>>? collection = null,
                IMongoQueryBuilder? queryBuilder = null)
        {
            var db = database ?? new Mock<IMongoDatabase>();
            var dbConnFactory = factory ?? new Mock<IMongoDatabaseConnectionFactory>();
            var coll = collection ?? new Mock<IMongoCollection<BeerEstablishment>>();
            var builder = queryBuilder ?? new Mock<IMongoQueryBuilder>().Object;
            dbConnFactory.Setup(f => f.ConnectToDatabase())
                .Returns(db.Object);
            db.Setup(d => d.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);

            return new MongoDbFindMeBeerService(dbConnFactory.Object, builder);
        }

        [Test]
        public async Task GetNearestBeerLocations_RetrievesConnectionToDatabase_FromDatabaseConnectionFactory()
        {
            var dbConnFactory = new Mock<IMongoDatabaseConnectionFactory>();

            var svc = CreateBeerService(dbConnFactory);
            await svc.GetNearestBeerLocations(new Location());

            dbConnFactory.Verify(f => f.ConnectToDatabase());
        }

        [Test]
        public async Task GetNearestBeerLocations_RetrievesVenuesCollection_FromDatabase()
        {
            var database = new Mock<IMongoDatabase>();

            var svc = CreateBeerService(database: database);
            await svc.GetNearestBeerLocations(new Location());

            database.Verify(f => f.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_BuildAggregation()
        {
            var builder = new Mock<IMongoQueryBuilder>();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.BuildPipeline());
        }

        [Test]
        public async Task GetNearestBeerLocations_PassesPipeline_ToCollection_Aggregate()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();
            var builder = new Mock<IMongoQueryBuilder>();
            var pipeline = (PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation>)Array.Empty<BsonDocument>();
            builder.Setup(b => b.BuildPipeline())
                .Returns(pipeline);

            var svc = CreateBeerService(collection: collection, queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            collection.Verify(c => c.Aggregate(pipeline, It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()));
        }

        //TODO : Tidy this up
        [Test]
        public async Task GetNearestBeerLocations_Returns_List_FromCollectionAggregate()
        {
            var collection = new Mock<IMongoCollection<BeerEstablishment>>();
            var cursor = new Mock<IAsyncCursor<BeerEstablishmentLocation>>();
            collection.Setup(c => c.Aggregate<BeerEstablishmentLocation>(It.IsAny<PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation>>(), 
                It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .Returns(cursor.Object);
            BeerEstablishmentLocation[] value = new[] { new BeerEstablishmentLocation() };
            cursor.Setup(c => c.Current)
                .Returns(value);
            cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            var svc = CreateBeerService(collection: collection);
            var locations = await svc.GetNearestBeerLocations(new Location());

            Assert.That(locations.FirstOrDefault(), Is.EqualTo(value.First()));
        }
    }


    public class MongoQueryBuilderTests
    {
        [Test]
        public void CreateLimitStage_CreatesDocWithOperationNameOf_Limit()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateLimitStage(10);

            Assert.That(doc.Names.First(), Is.EqualTo("$limit"));
        }

        [Test]
        public void CreateLimitStage_CreatesLimitDocWithPageSize()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateLimitStage(10);

            Assert.That(doc.Values.First().ToInt32(), Is.EqualTo(10));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_ForGivenFieldName()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateNotEqualQuery("Category", "Closed venues");

            Assert.That(doc.Names.First(), Is.EqualTo("Category"));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_WithOperationNameOf_NE()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateNotEqualQuery("Category", "Closed venues");

            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First(), Is.EqualTo("$ne"));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_WithFieldNameAndValueSet()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateNotEqualQuery("Category", "Closed venues");

            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Values.First().ToString(), Is.EqualTo("Closed venues"));
        }

        [Test]
        public void CreateProjectionStage_CreatesProjectDocWithFieldValuesSet()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateProjectionStage(new[] {"Name", "Distance"});

            Assert.That(doc.Names.First(), Is.EqualTo("$project"));
            var valueDoc = doc.Values.First().AsBsonDocument;
            
            Assert.That(valueDoc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(valueDoc.Values.ElementAt(0).AsInt32, Is.EqualTo(1));

            Assert.That(valueDoc.Names.ElementAt(1), Is.EqualTo("Distance"));
            Assert.That(valueDoc.Values.ElementAt(1).AsInt32, Is.EqualTo(1));
        }

        [Test]
        public void CreateProjectionStage_ExcludeIdTrue_CreatesProjectDocWithIdSetTo0()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateProjectionStage(new[] { "Name"}, true);

            var valueDoc = doc.Values.First().AsBsonDocument;

            Assert.That(valueDoc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(valueDoc.Values.ElementAt(0).AsInt32, Is.EqualTo(1));

            Assert.That(valueDoc.Names.ElementAt(1), Is.EqualTo("_id"));
            Assert.That(valueDoc.Values.ElementAt(1).AsInt32, Is.EqualTo(0));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithOperationNameOfGeoNear()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateGeoNearDocument(0, 0, string.Empty, string.Empty);

            Assert.That(doc.Names.First(), Is.EqualTo("$geoNear"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithValueDoc_OfNear()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateGeoNearDocument(0, 0, string.Empty, string.Empty);

            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First().ToString(), Is.EqualTo("near"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithNearValue_DefiningTargetCoordinates()
        {
            var builder = new MongoQueryBuilder();
            var lng = -1.555570961376415;
            var lat = 53.801196991070164;
            var doc = builder.CreateGeoNearDocument(lng, lat, string.Empty, string.Empty);

            var valueDoc = doc.Values.First().AsBsonDocument;
            var nearValueDoc = valueDoc.Values.First().AsBsonDocument;

            Assert.That(nearValueDoc.Names.ElementAt(0), Is.EqualTo("type"));
            Assert.That(nearValueDoc.Values.ElementAt(0).ToString(), Is.EqualTo("Point"));

            Assert.That(nearValueDoc.Names.ElementAt(1), Is.EqualTo("coordinates"));
            var coordsArray = nearValueDoc.Values.ElementAt(1).AsBsonArray;
            Assert.That(coordsArray.ElementAt(0).AsDouble, Is.EqualTo(lng));
            Assert.That(coordsArray.ElementAt(1).AsDouble, Is.EqualTo(lat));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithKeyDoc_ContainingNameOfCoordinateField()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateGeoNearDocument(0, 0, "Location.Coordinates", string.Empty);

            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("key");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("Location.Coordinates"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithDistanceFieldDoc_ContainingNameOfDistanceField()
        {
            var builder = new MongoQueryBuilder();
            var doc = builder.CreateGeoNearDocument(0, 0, string.Empty, "DistanceInMetres");

            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("distanceField");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("DistanceInMetres"));
        }

        [Test]
        public void TestPipeline()
        {
            var builder = new MongoQueryBuilder();
            var pipeLine = builder.BuildPipeline();

            var document = pipeLine.ToBsonDocument();
        }
    }
}
