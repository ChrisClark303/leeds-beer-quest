using LeedsBeerQuest.App.Models;
using LeedsBeerQuest.App.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LeedsBeerQuest.App.Services
{
    public class FindMeBeerService : IFindMeBeerService
    {
        private readonly IMemoryCache _cache;
        private readonly ILocationDistanceCalculator _distanceCalculator;
        private readonly FindMeBeerServiceSettings _settings;

        public FindMeBeerService(IMemoryCache cache, ILocationDistanceCalculator distanceCalculator, IOptions<FindMeBeerServiceSettings> settings)
        {
            _cache = cache;
            _distanceCalculator = distanceCalculator;
            _settings = settings.Value;
        }

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation = default)
        {
            var establishments = _cache.Get<BeerEstablishment[]>("establishments");
            if (establishments == null)
            {
                return Array.Empty<BeerEstablishmentLocation>();
            }

            var locations = CreateLocationModel(myLocation, establishments!);
            return locations
                .OrderBy(l => l.Distance)
                .Take(_settings.DefaultPageSize)
                .ToArray();
        }

        private IEnumerable<BeerEstablishmentLocation> CreateLocationModel(Location myLocation, BeerEstablishment[] establishments)
        {
            var startLocation = myLocation.Equals(default(Location)) ? _settings.DefaultSearchLocation : myLocation;
            var locations = establishments.Select(e => new BeerEstablishmentLocation()
            {
                Name = e.Name,
                Location = e.Location,
                Distance = _distanceCalculator.CalculateDistanceInMiles(startLocation, e.Location)
            });
            return locations;
        }
    }
}
