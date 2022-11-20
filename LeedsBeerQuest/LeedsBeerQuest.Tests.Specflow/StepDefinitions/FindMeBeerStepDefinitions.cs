using Gherkin.CucumberMessages.Types;
using LeedsBeerQuest.Tests.Specflow.Drivers;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
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

        [Given(@"I do not provide a location")]
        public async Task GivenIDoNotProvideALocation()
        {
            var dataImported = await _driver.ImportData();
            Assert.True(dataImported);
        }

        [When(@"I request the nearest beers establishments")]
        public async Task WhenIRequestTheNearestBeersEstablishments()
        {
            var gotBeer = await _driver.GetBeerEstablishments();
            Assert.True(gotBeer);
        }

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
    }
}