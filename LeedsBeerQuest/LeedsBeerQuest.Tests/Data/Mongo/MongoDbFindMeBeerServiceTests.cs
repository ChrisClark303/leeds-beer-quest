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
            coll.Setup(c => c.Aggregate<BeerEstablishmentLocation>(
                    It.IsAny<PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation>>(),
                    It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .Returns(new Mock<IAsyncCursor<BeerEstablishmentLocation>>().Object);
            coll.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BeerEstablishment>>(), It.IsAny<FindOptions<BeerEstablishment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<IAsyncCursor<BeerEstablishment>>().Object);
            return coll;
        }

        private Mock<IMongoQueryBuilder> CreateMockQueryBuilder(BsonDocument[]? builtDocs = null)
        {
            var queryBuilder = new Mock<IMongoQueryBuilder>();
            //ensure any fluent method returning an IMongoQueryBuilder instance returns the mock
            queryBuilder.SetReturnsDefault<IMongoQueryBuilder>(queryBuilder.Object);
            queryBuilder.Setup(q => q.Build())
                .Returns(builtDocs ?? new[] { new BsonDocument(), new BsonDocument() });
            return queryBuilder;
        }

        private Mock<IAsyncCursor<TResult>> CreateMockResultsCursor<TResult>(TResult[] results)
        {
            var cursor = new Mock<IAsyncCursor<TResult>>();
            cursor.Setup(c => c.Current)
                .Returns(results);
            cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            return cursor;
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
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_WithAggregationGeoNear_WithLocationDetails()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location() { Long = -1, Lat = 51});

            builder.Verify(b => b.WithAggregationGeoNear(-1, 51, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_NoLocationSpecified_Calls_QueryBuilder_WithAggregationGeoNear_WithDefaultSearchLocation()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object, defaultSearchLocation: new Location() { Long = -2, Lat = 52 });
            await svc.GetNearestBeerLocations(default);

            builder.Verify(b => b.WithAggregationGeoNear(-2, 52, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_WithAggregationGeoNear_WithFieldNames()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.WithAggregationGeoNear(It.IsAny<double>(), It.IsAny<double>(), "Location.Coordinates", "DistanceInMetres"));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_WithAggregationProjection_Include_WithFieldNames()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            string[] targetFieldNames = new[] { "Name", "Location.Lat", "Location.Long", "DistanceInMetres" };
            builder.Verify(b => b.WithAggregationProjection(It.Is<string[]>(f =>Is.EquivalentTo(targetFieldNames).ApplyTo(f).IsSuccess), ProjectionType.Include, It.IsAny<bool>()));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_WithAggregationProjection_ExcludingId()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetNearestBeerLocations(new Location());

            string[] targetFieldNames = new[] { "Name", "Location", "DistanceInMetres" };
            builder.Verify(b => b.WithAggregationProjection(It.IsAny<string[]>(), It.IsAny<ProjectionType>(), true));
        }

        [Test]
        public async Task GetNearestBeerLocations_Calls_QueryBuilder_WithAggregationLimit_WithDefaultPageSize()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object, pageSize: 7);
            await svc.GetNearestBeerLocations(new Location());

            builder.Verify(b => b.WithAggregationLimit(7));
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
            BeerEstablishmentLocation[] results = new[] { new BeerEstablishmentLocation() };
            var cursor = CreateMockResultsCursor(results);

            var collection = new Mock<IMongoCollection<BeerEstablishment>>();
            collection.Setup(c => c.Aggregate<BeerEstablishmentLocation>(It.IsAny<PipelineDefinition<BeerEstablishment, BeerEstablishmentLocation>>(), 
                It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .Returns(cursor.Object);
            
            var svc = CreateBeerService(collection: collection);
            var locations = await svc.GetNearestBeerLocations(new Location());

            Assert.That(locations, Is.EquivalentTo(results));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_RetrievesConnectionToDatabase_FromDatabaseConnectionFactory()
        {
            var dbConnFactory = new Mock<IMongoDatabaseConnectionFactory>();

            var svc = CreateBeerService(dbConnFactory);
            await svc.GetBeerEstablishmentByName(string.Empty);

            dbConnFactory.Verify(f => f.ConnectToDatabase());
        }

        [Test]
        public async Task GetBeerEstablishmentByName_RetrievesVenuesCollection_FromDatabase()
        {
            var database = new Mock<IMongoDatabase>();

            var svc = CreateBeerService(database: database);
            await svc.GetBeerEstablishmentByName(string.Empty);

            database.Verify(f => f.GetCollection<BeerEstablishment>("Venues", It.IsAny<MongoCollectionSettings>()));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_Calls_QueryBuilder_WithProjection_Exclude_WithFieldNames()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetBeerEstablishmentByName(string.Empty);

            string[] targetFieldNames = new[] { "Location._t", "Location.Coordinates", "_id" };
            builder.Verify(b => b.WithProjection(It.Is<string[]>(f => Is.EquivalentTo(targetFieldNames).ApplyTo(f).IsSuccess), ProjectionType.Exclude, It.IsAny<bool>()));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_Calls_QueryBuilder_WithEqualQuery()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetBeerEstablishmentByName("The Faversham");

            builder.Verify(b => b.WithIsEqualToQuery("Name", "The Faversham"));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_Calls_QueryBuilder_Build()
        {
            var builder = CreateMockQueryBuilder();

            var svc = CreateBeerService(queryBuilder: builder.Object);
            await svc.GetBeerEstablishmentByName(string.Empty);

            builder.Verify(b => b.Build());
        }

        [Test]
        public async Task GetBeerEstablishmentByName_CallsCollection_FindAsync_WithBuilder_FirstDoc()
        {
            var docs = new[] { new BsonDocument(), new BsonDocument() };
            var builder = CreateMockQueryBuilder(docs);
            var collection = CreateMockCollection();

            var svc = CreateBeerService(collection: collection, queryBuilder: builder.Object);
            await svc.GetBeerEstablishmentByName(string.Empty);

            collection.Verify(c => c.FindAsync(It.Is<BsonDocumentFilterDefinition<BeerEstablishment>>(b => b.Document == docs[0]), It.IsAny<FindOptions<BeerEstablishment>>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_CallsCollection_FindAsync_WithProjectionOptions_FromSecondDoc()
        {
            var docs = new[] { new BsonDocument(), new BsonDocument() };
            var builder = CreateMockQueryBuilder(docs);
            var collection = CreateMockCollection();

            var svc = CreateBeerService(collection: collection, queryBuilder: builder.Object);
            await svc.GetBeerEstablishmentByName(string.Empty);

            collection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<BeerEstablishment>>(), It.Is<FindOptions<BeerEstablishment>>(o => (o.Projection as BsonDocumentProjectionDefinition<BeerEstablishment,BeerEstablishment>).Document == docs[1]), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_Returns_Results_FromFindAsync()
        {
            var results = new[] { new BeerEstablishment() };
            var cursor = CreateMockResultsCursor(results);
            var collection = CreateMockCollection();
            collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BeerEstablishment>>(), It.IsAny<FindOptions<BeerEstablishment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var svc = CreateBeerService(collection: collection);
            var establishment = await svc.GetBeerEstablishmentByName(string.Empty);

            Assert.That(establishment, Is.EqualTo(results.First()));
        }
    }
}
