using LeedsBeerQuest.Api;
using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});

//app.MapControllers();
app.MapPatch("/data-management/import", async ([FromServices] DataImporter importer) =>
{
    await importer.Import();
});
//app.MapGet("/beer/nearest-locations", async ([FromServices] IFindMeBeerService beerService, double? lat, double? lng) =>
//{
//    //TODO : Move this into a controller since there is additional logic here.
//    Location? location = null;
//    if (lat != null && lng != null)
//    {
//        location = new Location() { Lat = (double)lat, Long = (double)lng }; 
//    }
//    return await beerService.GetNearestBeerLocations(location);
//});
app.MapGet("/beer/{establishmentName}", async ([FromServices] IFindMeBeerService beerService, string establishmentName) =>
{
    return await beerService.GetBeerEstablishmentByName(establishmentName);
});

app.Run();
