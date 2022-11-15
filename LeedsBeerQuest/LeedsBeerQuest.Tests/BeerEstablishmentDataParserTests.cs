using LeedsBeerQuest.Api;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests
{
    public class BeerEstablishmentDataParserTests
    {
        [Test]
        public void Parse_CorrectlyMapsModelProperties_FromCsvRow()
        {
            string allData = "\"name\",\"category\",\"url\",\"date\",\"excerpt\",\"thumbnail\",\"lat\",\"lng\",\"address\",\"phone\",\"twitter\",\"stars_beer\",\"stars_atmosphere\",\"stars_amenities\",\"stars_value\",\"tags\"\r\n" +
                "\"...escobar\",\"Closed venues\",\"http://leedsbeer.info/?p=765\",\"2012-11-30T21:58:52+00:00\",\"...It's really dark in here!\"," +
                "\"http://leedsbeer.info/wp-content/uploads/2012/11/20121129_185815.jpg\",\"53.8007317\",\"-1.5481764\"," +
                "\"23-25 Great George Street, Leeds LS1 3BB\",\"0113 220 4389\",\"EscobarLeeds\",\"2.2\",\"3\",\"3\",\"3.9\"," +
                "\"food,live music,sofas\"";

            var parser = new BeerEstablishmentDataParser();
            var establishments = parser.Parse(allData);

            var establishment = establishments.First();
            Assert.Multiple(() =>
            {
                Assert.That(establishment.Name, Is.EqualTo("...escobar"));
                Assert.That(establishment.Category, Is.EqualTo("Closed venues"));
                Assert.That(establishment.Url?.OriginalString, Is.EqualTo("http://leedsbeer.info/?p=765"));
                Assert.That(establishment.Date, Is.EqualTo(DateTime.Parse("2012-11-30T21:58:52+00:00")));
                Assert.That(establishment.Excerpt, Is.EqualTo("...It's really dark in here!"));
                Assert.That(establishment.Thumbnail?.OriginalString, Is.EqualTo("http://leedsbeer.info/wp-content/uploads/2012/11/20121129_185815.jpg"));
                Assert.That(establishment.Location.Lat, Is.EqualTo(53.8007317));
                Assert.That(establishment.Location.Long, Is.EqualTo(-1.5481764));
                Assert.That(establishment.Address, Is.EqualTo("23-25 Great George Street, Leeds LS1 3BB"));
                Assert.That(establishment.Phone, Is.EqualTo("0113 220 4389"));
                Assert.That(establishment.Twitter, Is.EqualTo("EscobarLeeds"));
                Assert.That(establishment.Ratings.Beer, Is.EqualTo(2.2));
                Assert.That(establishment.Ratings.Atmosphere, Is.EqualTo(3));
                Assert.That(establishment.Ratings.Amenities, Is.EqualTo(3));
                Assert.That(establishment.Ratings.Value, Is.EqualTo(3.9));
                Assert.That(establishment.Tags, Is.EquivalentTo(new[] { "food", "live music", "sofas" }));
            });
        }

        [Test]
        public void Parse_CreatesModelsForEveryDataRow()
        {
            string allData = "\"name\",\"category\",\"url\",\"date\",\"excerpt\",\"thumbnail\",\"lat\",\"lng\",\"address\",\"phone\",\"twitter\",\"stars_beer\",\"stars_atmosphere\",\"stars_amenities\",\"stars_value\",\"tags\"\r\n" +
                "\"115 The Headrow\",\"Pub reviews\",\"http://leedsbeer.info/?p=2753\",\"2014-10-18T15:48:51+00:00\",\"A bar that lives up to its name.\",\"http://leedsbeer.info/wp-content/uploads/2014/10/115.jpg\",\"53.7994003\",\"-1.545981\",\"115 The Headrow, Leeds, LS1 5JW\",\"\",\"BLoungeGrp\",\"1.5\",\"3\",\"2.5\",\"2\",\"coffee,food\"\r\n" +
                "\"1871 Bar & Lounge\",\"Bar reviews\",\"http://leedsbeer.info/?p=1455\",\"2013-05-16T16:10:30+00:00\",\"Not much going on here beer-wise, but they do serve food on a Sunday evening — quite a rarity amongst city centre bars. \",\"http://leedsbeer.info/wp-content/uploads/2013/05/20130506_205930.jpg\",\"53.7958908\",\"-1.5433109\",\"7-9 Boar Lane, Leeds LS1 5DD\",\"0845 200 1871\",\"1871BarLeeds\",\"1.5\",\"3\",\"2.5\",\"1.5\",\"coffee,food,free wifi\"\r\n" +
                "\"314 In Progress\",\"Bar reviews\",\"http://leedsbeer.info/?p=3048\",\"2015-03-24T20:31:30+00:00\",\"This fashionable cocktail bar & club isn't really our scene but has surprisingly good beer.\",\"http://leedsbeer.info/wp-content/uploads/2015/03/IMG_20150209_175103005.jpg\",\"53.8007202\",\"-1.5483344\",\"25 Great George Street, Leeds LS1 3AL\",\"0113 397 1337\",\"314Leeds\",\"3\",\"2.5\",\"1.5\",\"2\",\"dance floor\"";

            var parser = new BeerEstablishmentDataParser();
            var establishments = parser.Parse(allData);

            Assert.Multiple(() =>
            {
                Assert.That(establishments[0].Name, Is.EqualTo("115 The Headrow"));
                Assert.That(establishments[1].Name, Is.EqualTo("1871 Bar & Lounge"));
                Assert.That(establishments[2].Name, Is.EqualTo("314 In Progress"));
            });
        }

        [Test]
        public void Parse_DoesNotCreateTagArrayWithEmptyString_IfEstablishment_DoesNotHaveTags()
        {
            string allData = "\"name\",\"category\",\"url\",\"date\",\"excerpt\",\"thumbnail\",\"lat\",\"lng\",\"address\",\"phone\",\"twitter\",\"stars_beer\",\"stars_atmosphere\",\"stars_amenities\",\"stars_value\",\"tags\"\r\n" +
                "\"Vice & Virtue\",\"Bar reviews\",\"http://leedsbeer.info/?p=3453\",\"2016-03-17T21:32:12+00:00\",\"There's a new strip.. I mean cocktail bar in town.\",\"http://leedsbeer.info/wp-content/uploads/2016/03/IMG_20160309_213352921.jpg\",\"53.8005676\",\"-1.5406334\",\"68 New Briggate, Leeds LS1 6NU\",\"\",\"vandvleeds\",\"2.5\",\"3.5\",\"1.5\",\"3\",\"\"";

            var parser = new BeerEstablishmentDataParser();
            var establishments = parser.Parse(allData);

            Assert.IsEmpty(establishments.First().Tags);
        }
    }
}
