using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.App
{
    public class InMemoryDataManagementServiceTests
    {
        [Test]
        public void ImportData_InsertsObjects_IntoCache()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new InMemoryDataManagementService(cache);

            var data = new[] { new BeerEstablishment() };
            service.ImportData(data);

            var cachedData = cache.Get<BeerEstablishment[]>("establishments");
            Assert.That(cachedData, Is.EqualTo(data));
        }
    }
}
