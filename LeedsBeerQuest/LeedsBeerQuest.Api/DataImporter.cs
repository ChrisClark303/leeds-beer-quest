using LeedsBeerQuest.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LeedsBeerQuest.Api
{
    public class DataImporter
    {
        private readonly HttpClient _dataImportClient;
        private readonly IBeerEstablishmentDataParser _dataParser;
        private readonly IDataManagementService _dataManagementService;

        public DataImporter(HttpClient dataImportClient, IBeerEstablishmentDataParser dataParser, IDataManagementService dataManagementService)
        {
            _dataImportClient = dataImportClient;
            _dataParser = dataParser;
            _dataManagementService = dataManagementService;
        }

        public async Task Import()
        {
            var csvData = await _dataImportClient.GetAsync("leedsbeerquest.csv");
            var dataString = await csvData.Content.ReadAsStringAsync();

            var data = _dataParser.Parse(dataString);

            _dataManagementService.ImportData(data);
        }
    }

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
