using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Settings;
using LeedsBeerQuest.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace LeedsBeerQuest.Tests.Data.Mongo
{
    public class MongoDbFindMeBeerServiceTests
    {
        private MongoDbFindMeBeerService CreateBeerService(Mock<IMongoDatabaseConnectionFactory>? factory = null,
                Mock<IMongoDatabase>? database = null,
                Mock<IMongoCollection<BeerEstablishment>>? collection = null,
                IMongoQueryBuilder? queryBuilder = null,
                int pageSize = 5, Location? defaultSearchLocation = null)
        {
            var db = database ?? new Mock<IMongoDatabase>();
            var dbConnFactory = factory ?? new Mock<IMongoDatabaseConnectionFactory>();
            var coll = collection ?? CreateMockCollection();
            var builder = queryBuilder ?? CreateMockQueryBuilder().Object;
            dbConnFactory.Setup(f => f.ConnectToDatabase())
                .Returns(db.Object);
            db.Setup(d => d.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);

            var settings = Options.Create(new FindMeBeerServiceSettings() { DefaultPageSize = pageSize, DefaultSearchLocation = defaultSearchLocation });
            return new MongoDbFindMeBeerService(dbConnFactory.Object, builder, settings);
        }

        private Mock<IMongoCollection<BeerEstablishment>> CreateMockCollection()
        {
            var coll = new Mock<IMongoCollection<BeerEstablishment>>();
            coll.Setup(c => c.Aggregate<BeerEstablishmentLocation>(It.IsAny<PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation>>(),
                It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .Returns(new Mock<IAsyncCursor<BeerEstablishmentLocation>>().Object);
            return coll;
        }

        private Mock<IMongoQueryBuilder> CreateMockQueryBuilder()
        {
            var queryBuilder = new Mock<IMongoQueryBuilder>();
            queryBuilder.Setup(q => q.CreateGeoNearDocument(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(queryBuilder.Object);
            queryBuilder.Setup(q => q.CreateProjectionStage(It.IsAny<string[]>(), It.IsAny<bool>()))
                .Returns(queryBuilder.Object);
            queryBuilder.Setup(q => q.CreateLimitStage(It.IsAny<int>()))
                .Returns(queryBuilder.Object);
            return queryBuilder;
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
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_CreateGeoNearDocument_WithLocationDetails()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location() { Long = -1, Lat = 51});

            builder.Verify(b => b.CreateGeoNearDocument(-1, 51, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_NoLocationSpecified_Calls_QueryBuilder_CreateGeoNearDocument_WithDefaultSearchLocation()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object, defaultSearchLocation: new Location() { Long = -2, Lat = 52 });
            await svc.GetNearestBeerLocations(default);

            builder.Verify(b => b.CreateGeoNearDocument(-2, 52, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_CreateGeoNearDocument_WithFieldNames()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.CreateGeoNearDocument(It.IsAny<double>(), It.IsAny<double>(), "Location.Coordinates", "DistanceInMetres"));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_CreateProjectionStage_WithFieldNames()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            string[] targetFieldNames = new[] { "Name", "Location.Lat", "Location.Long", "DistanceInMetres" };
            builder.Verify(b => b.CreateProjectionStage(It.Is<string[]>(f =>Is.EquivalentTo(targetFieldNames).ApplyTo(f).IsSuccess), It.IsAny<bool>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_CreateProjectionStage_ExcludingId()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            string[] targetFieldNames = new[] { "Name", "Location", "DistanceInMetres" };
            builder.Verify(b => b.CreateProjectionStage(It.IsAny<string[]>(), true));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_CreateLimitStage_WithDefaultPageSize()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object, pageSize: 7);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.CreateLimitStage(7));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_BuildPipeline()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.BuildPipeline());
        }

        [Test]
        public async Task GetNearestBeerLocations_PassesPipeline_ToCollection_Aggregate()
        {
            var collection = CreateMockCollection();
            var builder = CreateMockQueryBuilder();
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
            BeerEstablishmentLocation[] results = new[] { new BeerEstablishmentLocation() };
            cursor.Setup(c => c.Current)
                .Returns(results);
            cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            var svc = CreateBeerService(collection: collection);
            var locations = await svc.GetNearestBeerLocations(new Location());

            Assert.That(locations, Is.EquivalentTo(results));
        }
    }
}
