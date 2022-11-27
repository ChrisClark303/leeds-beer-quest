using BoDi;
using LeedsBeerQuest.Api;
using LeedsBeerQuest.App;
using LeedsBeerQuest.App.Settings;
using LeedsBeerQuest.Tests.Specflow.Drivers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Specflow.Support
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void RegisterServices()
        {
            var factory = GetWebApplicationFactory();
            _objectContainer.RegisterInstanceAs(factory);
            _objectContainer.RegisterTypeAs<FindMeBeerApiDriver,IFindMeBeerApiDriver>();
        }

        private WebApplicationFactory<Program> GetWebApplicationFactory() =>
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.Configure<FindMeBeerServiceSettings>(s => s.DefaultPageSize = 10);
                        services.AddHttpClient<DataImporter>(client =>
                        {
                            client.BaseAddress = new Uri("http://localhost/test");
                        })
                        //This prevents the test from constantly downloading the CSV file from
                        //data mill north.
                        .AddHttpMessageHandler(() => StubMessageHandlerFactory.Create());
                    });
                });
    }
}
