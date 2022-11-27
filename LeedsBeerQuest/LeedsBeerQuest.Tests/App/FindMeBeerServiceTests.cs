using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using LeedsBeerQuest.App.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.App
{
    internal class FindMeBeerServiceTests
    {
        [Test]
        public async Task GetNearestBeerLocations_CacheReturnsNull_ReturnsEmptyArray()
        {
            var memCache = new MemoryCache(new MemoryCacheOptions());

            var findMeBeer = CreateService(memCache);
            var establishments = await findMeBeer.GetNearestBeerLocations(new Location());

            Assert.That(establishments, Is.Empty);
        }

        [Test]
        public async Task GetNearestBeerLocations_CalculatesDistance_ToEachLocation()
        {
            var location1 = new Location() { Lat = 1, Long = 2 };
            var location2 = new Location() { Lat = 3, Long = 4 };
            var location3 = new Location() { Lat = 5, Long = 6 };
            var memCache = new MemoryCache(new MemoryCacheOptions());
            memCache.Set("establishments", new[]
            {
                new BeerEstablishment() { Name = "establishment1", Location = location1},
                new BeerEstablishment() { Name = "establishment2", Location = location2},
                new BeerEstablishment() { Name = "establishment3", Location = location3}
            });
            var distanceCalculator = new Mock<ILocationDistanceCalculator>();

            var findMeBeer = CreateService(memCache, distanceCalculator.Object);
            Location myLocation = new Location() { Lat = 10, Long = 11 };
            var establishments = await findMeBeer.GetNearestBeerLocations(myLocation);

            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location1));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location2));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location3));
        }

        [Test]
        public async Task GetNearestBeerLocations_MyLocationIsDefault_UsesDefaultSearchLocationAsStartLocation()
        {
            var location1 = new Location() { Lat = 1, Long = 2 };
            var location2 = new Location() { Lat = 3, Long = 4 };
            var location3 = new Location() { Lat = 5, Long = 6 };
            var memCache = new MemoryCache(new MemoryCacheOptions());
            memCache.Set("establishments", new[]
            {
                new BeerEstablishment() { Name = "establishment1", Location = location1},
                new BeerEstablishment() { Name = "establishment2", Location = location2},
                new BeerEstablishment() { Name = "establishment3", Location = location3}
            });
            var distanceCalculator = new Mock<ILocationDistanceCalculator>();

            Location myLocation = new Location() { Lat = 10, Long = 11 };
            var findMeBeer = CreateService(memCache, distanceCalculator.Object, defaultSearchLocation: myLocation);

            var establishments = await findMeBeer.GetNearestBeerLocations(default);

            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location1));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location2));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMetres(myLocation, location3));
        }

        [Test]
        public async Task GetNearestBeerLocations_Returns_ResultsWithPageSizeFromSettings()
        {
            var locations = Enumerable.Range(0, 10).Select(i => new BeerEstablishment());
            var memCache = new MemoryCache(new MemoryCacheOptions());
            memCache.Set("establishments", locations.ToArray());

            var findMeBeer = CreateService(memCache, pageSize: 4);
            var establishments = await findMeBeer.GetNearestBeerLocations(new Location());

            Assert.That(establishments.Length, Is.EqualTo(4));
        }

        [Test]
        public async Task GetNearestBeerLocations_ReturnsNearestResults_InAscendingOrderOfDistanceInMetres()
        {
            var locations = Enumerable.Range(0, 5).Select(i => new BeerEstablishment());
            var memCache = new MemoryCache(new MemoryCacheOptions());
            memCache.Set("establishments", locations.ToArray());

            var distanceCalculator = new Mock<ILocationDistanceCalculator>();
            distanceCalculator.SetupSequence(c => c.CalculateDistanceInMetres(It.IsAny<Location>(), It.IsAny<Location>()))
                .Returns(5)
                .Returns(4)
                .Returns(3)
                .Returns(2)
                .Returns(1);

            var findMeBeer = CreateService(memCache, distanceCalculator.Object, pageSize: 4);
            var establishments = await findMeBeer.GetNearestBeerLocations(new Location());

            Assert.That(establishments.Select(e => e.DistanceInMetres), Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_CacheReturnsNull_ReturnsNull()
        {
            var memCache = new MemoryCache(new MemoryCacheOptions());

            var findMeBeer = CreateService(memCache);
            var establishment = await findMeBeer.GetBeerEstablishmentByName(string.Empty);

            Assert.That(establishment, Is.Null);
        }

        [Test]
        public async Task GetNearestBeerLocations_ReturnsEstablishmentWithSpecifiedName()
        {
            var memCache = new MemoryCache(new MemoryCacheOptions());
            var cachedEstablishments = new[]
            {
                new BeerEstablishment() { Name = "establishment1"},
                new BeerEstablishment() { Name = "establishment2"},
                new BeerEstablishment() { Name = "establishment3"}
            };
            memCache.Set("establishments", cachedEstablishments);
            
            var findMeBeer = CreateService(memCache);
            var establishment = await findMeBeer.GetBeerEstablishmentByName("establishment2");
            
            Assert.That(establishment, Is.EqualTo(cachedEstablishments[1]));
        }

        [Test]
        public async Task GetNearestBeerLocations_EstablishmentDoesNotExistInCache_ReturnsNull()
        {
            var memCache = new MemoryCache(new MemoryCacheOptions());
            var cachedEstablishments = new[]
            {
                new BeerEstablishment() { Name = "establishment1"},
                new BeerEstablishment() { Name = "establishment2"},
                new BeerEstablishment() { Name = "establishment3"}
            };
            memCache.Set("establishments", cachedEstablishments);

            var findMeBeer = CreateService(memCache);
            var establishment = await findMeBeer.GetBeerEstablishmentByName("establishment4");

            Assert.That(establishment, Is.Null);
        }

        private FindMeBeerService CreateService(IMemoryCache? memCache = null, ILocationDistanceCalculator? distanceCalc = null, int pageSize = 5, Location? defaultSearchLocation = null)
        {
            return new FindMeBeerService(memCache ?? new MemoryCache(new MemoryCacheOptions()),
                distanceCalc ?? new Mock<ILocationDistanceCalculator>().Object,
                Options.Create(new FindMeBeerServiceSettings() { DefaultPageSize = pageSize, DefaultSearchLocation = defaultSearchLocation }));
        }
    }
}
