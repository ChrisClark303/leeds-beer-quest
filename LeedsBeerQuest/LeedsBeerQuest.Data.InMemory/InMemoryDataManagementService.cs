using LeedsBeerQuest.App.Models.Read;
using Microsoft.Extensions.Caching.Memory;

namespace LeedsBeerQuest.App
{
    public class InMemoryDataManagementService : IDataManagementService
    {
        private readonly IMemoryCache _memCache;

        public InMemoryDataManagementService(IMemoryCache memCache)
        {
            _memCache = memCache;
        }

        public void ImportData(BeerEstablishment[] establishments)
        {
            _memCache.Set("establishments", establishments, TimeSpan.FromDays(7));
        }
    }
}
