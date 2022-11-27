using LeedsBeerQuest.Data.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace LeedsBeerQuest.Tests.Data.Mongo
{
    public class MongoQueryBuilderTests
    {
        [Test]
        public void WithAggregationLimit_CreatesDocWithOperationNameOf_Limit()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationLimit(10)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$limit"));
        }

        [Test]
        public void WithAggregationLimit_CreatesLimitDocWithPageSize()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationLimit(10)
            .Build();

            var doc = docs.First();
            Assert.That(doc.Values.First().ToInt32(), Is.EqualTo(10));
        }

        [Test]
        public void WithNotEqualQuery_CreatesDoc_ForGivenFieldName()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("Category"));
        }

        [Test]
        public void WithNotEqualQuery_CreatesDoc_WithOperationNameOf_NE()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First(), Is.EqualTo("$ne"));
        }

        [Test]
        public void WithNotEqualQuery_CreatesDoc_WithFieldNameAndValueSet()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Values.First().ToString(), Is.EqualTo("Closed venues"));
        }

        [Test]
        public void WithIsEqualToQuery_CreatesDoc_WithFieldNameAndValueSet()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithIsEqualToQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("Category"));
            Assert.That(doc.Values.First().ToString(), Is.EqualTo("Closed venues"));
        }

        [Test]
        public void WithAggregationProjection_ProjectionTypeInclude_CreatesProjectDocWithFieldValuesSet_To_1()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationProjection(new[] {"Name", "Distance"}, ProjectionType.Include)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$project"));
            var valueDoc = doc.Values.First().AsBsonDocument;
            
            Assert.That(valueDoc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(valueDoc.Values.ElementAt(0).AsInt32, Is.EqualTo(1));

            Assert.That(valueDoc.Names.ElementAt(1), Is.EqualTo("Distance"));
            Assert.That(valueDoc.Values.ElementAt(1).AsInt32, Is.EqualTo(1));
        }

        [Test]
        public void WithAggregationProjection_ProjectionTypeExclude_CreatesProjectDocWithFieldValuesSet_To_0()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationProjection(new[] { "Name", "Distance" }, ProjectionType.Exclude)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$project"));
            var valueDoc = doc.Values.First().AsBsonDocument;

            Assert.That(valueDoc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(valueDoc.Values.ElementAt(0).AsInt32, Is.EqualTo(0));

            Assert.That(valueDoc.Names.ElementAt(1), Is.EqualTo("Distance"));
            Assert.That(valueDoc.Values.ElementAt(1).AsInt32, Is.EqualTo(0));
        }

        [Test]
        public void WithAggregationProjection_ExcludeIdTrue_CreatesProjectDocWithIdSetTo0()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationProjection(new[] { "Name"}, ProjectionType.Include, true)
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;

            Assert.That(valueDoc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(valueDoc.Values.ElementAt(0).AsInt32, Is.EqualTo(1));

            Assert.That(valueDoc.Names.ElementAt(1), Is.EqualTo("_id"));
            Assert.That(valueDoc.Values.ElementAt(1).AsInt32, Is.EqualTo(0));
        }

        [Test]
        public void WithAggregationGeoNear_CreatesDocument_WithOperationNameOfGeoNear()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationGeoNear(0, 0, string.Empty, string.Empty)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$geoNear"));
        }

        [Test]
        public void WithAggregationGeoNear_CreatesDocument_WithValueDoc_OfNear()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationGeoNear(0, 0, string.Empty, string.Empty)
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First().ToString(), Is.EqualTo("near"));
        }

        [Test]
        public void WithAggregationGeoNear_CreatesDocument_WithNearValue_DefiningTargetCoordinates()
        {
            var builder = new MongoQueryBuilder();
            var lng = -1.555570961376415;
            var lat = 53.801196991070164;
            var docs = builder.WithAggregationGeoNear(lng, lat, string.Empty, string.Empty)
                .Build();

            var doc = docs.First();
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
        public void WithAggregationGeoNear_CreatesDocument_WithKeyDoc_ContainingNameOfCoordinateField()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationGeoNear(0, 0, "Location.Coordinates", string.Empty)
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("key");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("Location.Coordinates"));
        }

        [Test]
        public void WithAggregationGeoNear_CreatesDocument_WithDistanceFieldDoc_ContainingNameOfDistanceField()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithAggregationGeoNear(0, 0, string.Empty, "DistanceInMetres")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("distanceField");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("DistanceInMetres"));
        }

        [Test]
        public void WithProjection_ProjectionType_Include_CreatesDocWithFieldValuesSet_1()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithProjection(new[] { "Name", "Distance" }, ProjectionType.Include)
                .Build();

            var doc = docs.First();
            
            Assert.That(doc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(doc.Values.ElementAt(0).AsInt32, Is.EqualTo(1));

            Assert.That(doc.Names.ElementAt(1), Is.EqualTo("Distance"));
            Assert.That(doc.Values.ElementAt(1).AsInt32, Is.EqualTo(1));
        }

        [Test]
        public void WithProjection_ProjectionType_Exclude_CreatesDocWithFieldValuesSet_0()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithProjection(new[] { "Name", "Distance" }, ProjectionType.Exclude)
                .Build();

            var doc = docs.First();

            Assert.That(doc.Names.ElementAt(0), Is.EqualTo("Name"));
            Assert.That(doc.Values.ElementAt(0).AsInt32, Is.EqualTo(0));

            Assert.That(doc.Names.ElementAt(1), Is.EqualTo("Distance"));
            Assert.That(doc.Values.ElementAt(1).AsInt32, Is.EqualTo(0));
        }

        [Test]
        public void WithProjection_ExcludeIdTrue_CreatesProjectDocWithIdSetTo0()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.WithProjection(new[] { "Name" }, ProjectionType.Include, excludeId: true)
                .Build();

            var doc = docs.First();

            Assert.That(doc.Names.ElementAt(1), Is.EqualTo("_id"));
            Assert.That(doc.Values.ElementAt(1).AsInt32, Is.EqualTo(0));
        }

        [Test]
        public void BuildPipeline_BuildsPipelineWithDocumentsInTheCorrectOrder()
        {
            var builder = new MongoQueryBuilder();
            builder.WithAggregationGeoNear(0, 0, "", "");
            builder.WithAggregationProjection(new[] { "" }, ProjectionType.Include);
            builder.WithAggregationLimit(0);
            var pipeLine = builder.BuildPipeline();

            var firstStage = pipeLine.Stages.ElementAt(0);
            Assert.That(firstStage.OperatorName, Is.EqualTo("$geoNear"));
            var secondStage = pipeLine.Stages.ElementAt(1);
            Assert.That(secondStage.OperatorName, Is.EqualTo("$project"));
            var thirdStage = pipeLine.Stages.ElementAt(2);
            Assert.That(thirdStage.OperatorName, Is.EqualTo("$limit"));
        }
    }
}
