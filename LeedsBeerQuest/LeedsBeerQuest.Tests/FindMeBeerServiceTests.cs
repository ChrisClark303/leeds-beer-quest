using LeedsBeerQuest.Api;
using LeedsBeerQuest.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests
{
    internal class FindMeBeerServiceTests
    {
        [Test]
        public async Task GetNearestBeerLocations_CacheReturnsNull_ReturnsEmptyArray()
        {
            var memCache = new MemoryCache(new MemoryCacheOptions());

            var findMeBeer = new FindMeBeerService(memCache); 
            var establishments = await findMeBeer.GetNearestBeerLocations(new Location());

            Assert.That(establishments, Is.Empty);
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
