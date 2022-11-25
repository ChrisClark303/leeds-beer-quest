using LeedsBeerQuest.App;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LeedsBeerQuest.Api
{
    public static class WebApplicationExtensions
    {
        public static void MapMinimalApis(this WebApplication app)
        {
            app.MapPatch("/data-management/import", async ([FromServices] DataImporter importer) =>
            {
                await importer.Import();
            })
            .WithTags("Data Management")
            .WithMetadata(new SwaggerOperationAttribute(summary: "Builds beer establishment data", description: "Rebuilds the beer establishment data source by importing the data from LeedsBeerQuest and pushing into the configured data store."));
        }
    }
}
