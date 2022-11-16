using LeedsBeerQuest.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LeedsBeerQuest.Data
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
