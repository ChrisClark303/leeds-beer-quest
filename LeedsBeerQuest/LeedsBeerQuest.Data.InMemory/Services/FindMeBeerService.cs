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
            var establishments = GetEstablishmentsFromCache();
            var establishment = establishments?.FirstOrDefault(e => e.Name == establishmentName);

            return Task.FromResult(establishment);
        }

        public Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null)
        {
            var establishments = GetEstablishmentsFromCache();
            var nearestLocations = Array.Empty<BeerEstablishmentLocation>();
            if (establishments != null)
            {
                var startLocation = myLocation ?? _settings.DefaultSearchLocation!;
                nearestLocations = establishments
                    .Select(e => ConvertToLocationModel(e, startLocation))
                    .OrderBy(l => l.DistanceInMetres)
                    .Take(_settings.DefaultPageSize)
                    .ToArray();
            }

            return Task.FromResult(nearestLocations);
        }

        private BeerEstablishmentLocation ConvertToLocationModel(BeerEstablishment establishment, Location startLocation)
        {
            return new BeerEstablishmentLocation()
            {
                Name = establishment.Name,
                Location = establishment.Location,
                DistanceInMetres = _distanceCalculator.CalculateDistanceInMetres(startLocation, establishment.Location!)
            };
        }

        private BeerEstablishment[]? GetEstablishmentsFromCache()
        {
            return _cache.Get<BeerEstablishment[]>("establishments");
        }
    }
}
