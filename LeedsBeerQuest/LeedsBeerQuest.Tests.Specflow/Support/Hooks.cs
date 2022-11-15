using BoDi;
using LeedsBeerQuest.Tests.Specflow.Drivers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    //config overrides here
                });
    }
}
