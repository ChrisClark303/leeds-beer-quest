using LeedsBeerQuest.App.Models.Read;
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

        public Task<BeerEstablishment?> GetBeerEstablishmentByName(string establishmentName)
        {
            var establishments = _cache.Get<BeerEstablishment[]>("establishments");
            BeerEstablishment? establishment = null;
            if (establishments != null) //TODO : Invert this if?
            {
                establishment = establishments.FirstOrDefault(e => e.Name == establishmentName);
            }

            return Task.FromResult(establishment);
        }

        //TODO : Address this warning in the readme
        public Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null)
        {
            var establishments = _cache.Get<BeerEstablishment[]>("establishments");
            var nearestLocations = Array.Empty<BeerEstablishmentLocation>();
            if (establishments != null)
            {
                var locations = CreateLocationModel(myLocation, establishments!);
                nearestLocations = locations
                    .OrderBy(l => l.Distance)
                    .Take(_settings.DefaultPageSize)
                    .ToArray();
            }

            return Task.FromResult(nearestLocations);
        }

        private IEnumerable<BeerEstablishmentLocation> CreateLocationModel(Location? myLocation, BeerEstablishment[] establishments)
        {
            var startLocation = myLocation ?? _settings.DefaultSearchLocation!;
            var locations = establishments.Select(e => new BeerEstablishmentLocation()
            {
                Name = e.Name,
                Location = e.Location,
                Distance = _distanceCalculator.CalculateDistanceInMiles(startLocation, e.Location!)
            });
            return locations;
        }
    }
}
