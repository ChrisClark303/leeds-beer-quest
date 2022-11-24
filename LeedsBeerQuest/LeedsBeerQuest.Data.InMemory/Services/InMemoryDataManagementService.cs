using LeedsBeerQuest.App.Models.Read;
using Microsoft.Extensions.Caching.Memory;

namespace LeedsBeerQuest.App.Services
{
    public class InMemoryDataManagementService : IDataManagementService
    {
        private readonly IMemoryCache _memCache;

        public InMemoryDataManagementService(IMemoryCache memCache)
        {
            _memCache = memCache;
        }

        public Task ImportData(BeerEstablishment[] establishments)
        {
            _memCache.Set("establishments", establishments, TimeSpan.FromDays(7));
            return Task.CompletedTask;
        }
    }
}
