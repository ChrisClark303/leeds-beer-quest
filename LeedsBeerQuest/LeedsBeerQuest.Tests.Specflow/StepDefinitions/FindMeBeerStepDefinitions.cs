using Gherkin.CucumberMessages.Types;
using LeedsBeerQuest.Tests.Specflow.Drivers;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using TechTalk.SpecFlow.Bindings;

namespace LeedsBeerQuest.Tests.Specflow.StepDefinitions
{
    [Binding]
    public sealed class FindMeBeerStepDefinitions
    {
        private readonly IFindMeBeerApiDriver _driver;

        internal FindMeBeerStepDefinitions(IFindMeBeerApiDriver driver)
        {
            _driver = driver;
        }

        [BeforeStep]
        public async Task Setup()
        {
            var dataImported = await _driver.ImportData();
            Assert.True(dataImported);
        }

        [Given(@"I do not provide a location")]
        public void GivenIDoNotProvideALocation()
        {
            _driver.SetSearchLocation(null);
        }

        [Given(@"I provide a latitude of (.*) and a longitiude of (.*)")]
        public void GivenIProvideALatitudeAndLongitude(double lat, double lng)
        {
            _driver.SetSearchLocation(new App.Models.Read.Location() { Lat = lat, Long = lng });
        }

        [When(@"I request the nearest beers establishments")]
        public async Task WhenIRequestTheNearestBeersEstablishments()
        {
            var gotBeer = await _driver.GetBeerEstablishments();
            Assert.True(gotBeer);
        }

        [Then(@"the 10 establishments closest to that location should be returned")]
        [Then(@"the 10 establishments closest to Joseph's Well should be returned")]
        public void ThenTheEstablishmentsClosestToJosephsWellShouldBeReturned(Table table)
        {
            Assert.AreEqual(table.RowCount, _driver.Establishments.Length);
            for (int i = 0; i < table.RowCount; i++)
            {
                var row = table.Rows[i];
                var establishment = _driver.Establishments[i];
                Assert.AreEqual(row["Name"], establishment.Name);
                Assert.AreEqual(Double.Parse(row["Lat"]), establishment.Location?.Lat);
                Assert.AreEqual(Double.Parse(row["Lon"]), establishment.Location?.Long);
                Assert.AreEqual(Double.Parse(row["Distance"]), establishment.Distance, 0.001);
            }
        }

        [Given(@"I provide '(.*)' as the name of an establishment")]
        public void GivenIProvideAsTheNameOfAnEstablishment(string establishmentName)
        {
            _driver.SetSearchEstablishmentName(establishmentName);
        }

        [When(@"I request the establishment details")]
        public async Task WhenIRequestTheEstablishmentDetails()
        {
            var gotBeer = await _driver.GetBeerEstablishmentByName();
            Assert.True(gotBeer);
        }

        [Then(@"all details about that establishment should be returned")]
        public void ThenAllDetailsAboutThatEstablishmentShouldBeReturned(Table table)
        {
            var e = _driver.EstablishmentByName;
            var dataRow = table.Rows.First();
            dataRow[nameof(e.Name)] = e.Name;
            dataRow[nameof(e.Category)] = e.Category;
            dataRow[nameof(e.Location)] = $"{e.Location.Lat},{e.Location.Long}";
            dataRow[nameof(e.Address)] = e.Address;
            dataRow[nameof(e.Phone)] = e.Phone;
            dataRow[nameof(e.Twitter)] = e.Twitter;
            dataRow[nameof(e.Thumbnail)] = e.Thumbnail.ToString();
            dataRow[nameof(e.Excerpt)] = e.Excerpt;
            dataRow[nameof(e.Date)] = e.Date.ToString();
            dataRow[nameof(e.Tags)] = string.Join(",", e.Tags);
            dataRow[nameof(e.Ratings.Value)] = e.Ratings.Value.ToString();
            dataRow[nameof(e.Ratings.Amenities)] = e.Ratings.Amenities.ToString();
            dataRow[nameof(e.Ratings.Atmosphere)] = e.Ratings.Atmosphere.ToString();
            dataRow[nameof(e.Ratings.Beer)] = e.Ratings.Beer.ToString();
        }

        [Then(@"a (.*) response should be returned")]
        public void ThenAResponseShouldBeReturned(string responseType)
        {
            var statusCode = Enum.Parse<HttpStatusCode>(responseType);
            Assert.That(_driver.LastRequestStatusCode, Is.EqualTo(statusCode));
        }
    }
}