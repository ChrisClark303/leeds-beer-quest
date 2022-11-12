using LeedsBeerQuest.Api;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DataImporter>();
builder.Services.AddHttpClient<DataImporter>(client =>
{
    //TODO : Needs to come from config!!
    client.BaseAddress = new Uri("https://datamillnorth.org/download/leeds-beer-quest/c8884f6c-84a0-4a54-9c71-c5016bf4d878/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/data-management/import", async ([FromServices] DataImporter importer) =>
{
    await importer.Import();
});

app.Run();
