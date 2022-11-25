using LeedsBeerQuest.App.Services;
using System.Collections.Generic;

namespace LeedsBeerQuest.App
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
            await _dataManagementService.ImportData(parsedEstablishments);
        }

        private async Task<string> GetBeerQuestData()
        {
            var csvData = await _dataImportClient.GetAsync("leedsbeerquest.csv");
            return await csvData.Content.ReadAsStringAsync();
        }
    }
}
