using LeedsBeerQuest.Api;
using LeedsBeerQuest.Api.Models;
using LeedsBeerQuest.Api.Services;
using Moq;

namespace LeedsBeerQuest.Tests
{
    public class DataImporterTests
    {
        private DataImporter CreateDataImporter(StubMessageHandler? stubMessageHandler = null, IBeerEstablishmentDataParser? dataParser = null, IDataManagementService dataManagementService = null)
        {
            var httpClient = new HttpClient(stubMessageHandler ?? new StubMessageHandler()) { BaseAddress = new Uri("http://test_uri/") };
            return new DataImporter(httpClient, 
                dataParser ?? new Mock<IBeerEstablishmentDataParser>().Object, 
                dataManagementService ?? new Mock<IDataManagementService>().Object);
        }

        [Test]
        public async Task Import_RetrievesBeerQuestCsvData_FromHttpClient()
        {
            var stubMessageHandler = new StubMessageHandler();

            var dataImporter = CreateDataImporter(stubMessageHandler);
            await dataImporter.Import();

            Assert.That(stubMessageHandler.LastRequestUri!.OriginalString, Is.EqualTo("http://test_uri/leedsbeerquest.csv"));
        }

        [Test]
        public async Task Import_PassesClientResponse_ToParser()
        {
            var stubMessageHandler = StubMessageHandler.WithResponse(new HttpResponseMessage()
            {
                Content = new StringContent("test establishment data")
            });
            var parser = new Mock<IBeerEstablishmentDataParser>();

            var dataImporter = CreateDataImporter(stubMessageHandler, parser.Object);
            await dataImporter.Import();

            parser.Verify(p => p.Parse("test establishment data"));
        }

        [Test]
        public async Task Import_PassesParsedModels_To_DataManagementService_ImportData()
        {
            var parser = new Mock<IBeerEstablishmentDataParser>();
            BeerEstablishment[] establishments = new[] { new BeerEstablishment() };
            parser.Setup(p => p.Parse(It.IsAny<string>()))
                .Returns(establishments) ;
            var service = new Mock<IDataManagementService>();

            var dataImporter = CreateDataImporter(dataParser: parser.Object, dataManagementService: service.Object);
            await dataImporter.Import();

            service.Verify(s => s.ImportData(establishments));
        }
    }
}