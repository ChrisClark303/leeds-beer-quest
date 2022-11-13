using LeedsBeerQuest.Api.Models;
using LeedsBeerQuest.Api.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LeedsBeerQuest.Api
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

        public async Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation)
        {
            //get objects from cache
            var establishments = _cache.Get<BeerEstablishment[]>("establishments");
            if (establishments == null)
            {
                return Array.Empty<BeerEstablishmentLocation>();
            }
            //convert to the right model
            var locations = establishments.Select(e => new BeerEstablishmentLocation() { Name = e.Name, Location = e.Location });
            //order by distance
            //page

            return locations.Take(10).ToArray();
        }
    }
}
