using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LeedsBeerQuest.Api.Services;

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
            string dataString = await GetBeerQuestData();
            var parsedEstablishments = _dataParser.Parse(dataString);
            _dataManagementService.ImportData(parsedEstablishments);
        }

        private async Task<string> GetBeerQuestData()
        {
            var csvData = await _dataImportClient.GetAsync("leedsbeerquest.csv");
            var dataString = await csvData.Content.ReadAsStringAsync();
            return dataString;
        }
    }
}
