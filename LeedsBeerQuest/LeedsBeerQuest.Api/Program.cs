using LeedsBeerQuest.Api;
using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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

app.MapControllers();
app.MapPatch("/data-management/import", async ([FromServices] DataImporter importer) =>
{
    await importer.Import();
});
app.MapGet("/beer/nearest-locations", async ([FromServices] IFindMeBeerService beerService) =>
{
    //need to gather the location somehow!!
    return await beerService.GetNearestBeerLocations(default);
});

app.Run();
