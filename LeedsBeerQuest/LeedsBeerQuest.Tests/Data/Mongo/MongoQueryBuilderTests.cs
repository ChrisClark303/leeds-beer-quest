using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace LeedsBeerQuest.Tests.Data.Mongo
{
    public class MongoQueryBuilderTests
    {
        [Test]
        public void CreateLimitStage_CreatesDocWithOperationNameOf_Limit()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateLimitStage(10)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$limit"));
        }

        [Test]
        public void CreateLimitStage_CreatesLimitDocWithPageSize()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateLimitStage(10)
            .Build();

            var doc = docs.First();
            Assert.That(doc.Values.First().ToInt32(), Is.EqualTo(10));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_ForGivenFieldName()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("Category"));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_WithOperationNameOf_NE()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First(), Is.EqualTo("$ne"));
        }

        [Test]
        public void CreateNotEqualQuery_CreatesDoc_WithFieldNameAndValueSet()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateNotEqualQuery("Category", "Closed venues")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Values.First().ToString(), Is.EqualTo("Closed venues"));
        }

        [Test]
        public void CreateProjectionStage_CreatesProjectDocWithFieldValuesSet()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateProjectionStage(new[] {"Name", "Distance"})
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
        public void CreateProjectionStage_ExcludeIdTrue_CreatesProjectDocWithIdSetTo0()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateProjectionStage(new[] { "Name"}, true)
                .Build();

            var doc = docs.First();
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
            var docs = builder.CreateGeoNearDocument(0, 0, string.Empty, string.Empty)
                .Build();

            var doc = docs.First();
            Assert.That(doc.Names.First(), Is.EqualTo("$geoNear"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithValueDoc_OfNear()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateGeoNearDocument(0, 0, string.Empty, string.Empty)
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            Assert.That(valueDoc.Names.First().ToString(), Is.EqualTo("near"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithNearValue_DefiningTargetCoordinates()
        {
            var builder = new MongoQueryBuilder();
            var lng = -1.555570961376415;
            var lat = 53.801196991070164;
            var docs = builder.CreateGeoNearDocument(lng, lat, string.Empty, string.Empty)
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
        public void CreateGeoNearDocument_CreatesDocument_WithKeyDoc_ContainingNameOfCoordinateField()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateGeoNearDocument(0, 0, "Location.Coordinates", string.Empty)
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("key");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("Location.Coordinates"));
        }

        [Test]
        public void CreateGeoNearDocument_CreatesDocument_WithDistanceFieldDoc_ContainingNameOfDistanceField()
        {
            var builder = new MongoQueryBuilder();
            var docs = builder.CreateGeoNearDocument(0, 0, string.Empty, "DistanceInMetres")
                .Build();

            var doc = docs.First();
            var valueDoc = doc.Values.First().AsBsonDocument;
            var keyDocIdx = valueDoc.IndexOfName("distanceField");
            Assert.That(valueDoc.Values.ElementAt(keyDocIdx).ToString(), Is.EqualTo("DistanceInMetres"));
        }

        [Test]
        public void Build_BuildsPipelineWithDocumentsInTheCorrectOrder()
        {
            var builder = new MongoQueryBuilder();
            builder.CreateGeoNearDocument(0, 0, "", "");
            builder.CreateProjectionStage(new[] { "" });
            builder.CreateLimitStage(0);
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
