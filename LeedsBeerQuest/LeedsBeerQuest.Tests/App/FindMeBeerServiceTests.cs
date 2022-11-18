using LeedsBeerQuest.App.Models;
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
        private FindMeBeerService CreateService(IMemoryCache? memCache = null, ILocationDistanceCalculator? distanceCalc = null, int pageSize = 5, Location defaultSearchLocation = default)
        {
            return new FindMeBeerService(memCache ?? new MemoryCache(new MemoryCacheOptions()),
                distanceCalc ?? new Mock<ILocationDistanceCalculator>().Object,
                Options.Create(new FindMeBeerServiceSettings() { DefaultPageSize = pageSize, DefaultSearchLocation = defaultSearchLocation }));
        }

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

            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location1));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location2));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location3));
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

            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location1));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location2));
            distanceCalculator.Verify(dc => dc.CalculateDistanceInMiles(myLocation, location3));
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
        public async Task GetNearestBeerLocations_ReturnsNearestResults_InAscendingOrder()
        {
            var locations = Enumerable.Range(0, 5).Select(i => new BeerEstablishment());
            var memCache = new MemoryCache(new MemoryCacheOptions());
            memCache.Set("establishments", locations.ToArray());

            var distanceCalculator = new Mock<ILocationDistanceCalculator>();
            distanceCalculator.SetupSequence(c => c.CalculateDistanceInMiles(It.IsAny<Location>(), It.IsAny<Location>()))
                .Returns(5)
                .Returns(4)
                .Returns(3)
                .Returns(2)
                .Returns(1);

            var findMeBeer = CreateService(memCache, distanceCalculator.Object, pageSize: 4);
            var establishments = await findMeBeer.GetNearestBeerLocations(new Location());

            Assert.That(establishments.Select(e => e.Distance), Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }
    }

    internal class LocationDistanceCalculatorTests
    {
        //        Input : Latitude 1: 53.32055555555556
        //        Latitude 2: 53.31861111111111
        //        Longitude 1: -1.7297222222222221
        //        Longitude 2: -1.6997222222222223
        //        Output: Distance is: 2.0043678382716137 Kilometers / 1.609344

        [Test]
        public void CalculateDistanceInMiles_CorrectlyCalculatesDistanceBetweenTwoLocations()
        {
            var location1 = new Location() { Lat = 53.32055555555556, Long = -1.7297222222222221 };
            var location2 = new Location() { Lat = 53.31861111111111, Long = -1.6997222222222223 };

            var distanceCalculator = new LocationDistanceCalculator();
            var distance = distanceCalculator.CalculateDistanceInMiles(location1, location2);

            Assert.That(distance, Is.EqualTo(1.245));
        }
    }
}
