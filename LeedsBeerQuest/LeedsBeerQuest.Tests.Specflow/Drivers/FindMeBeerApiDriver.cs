using LeedsBeerQuest.App.Models.Read;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Specflow.Drivers
{
    internal class FindMeBeerApiDriver : IFindMeBeerApiDriver
    {
        private readonly HttpClient _client;

        public BeerEstablishmentLocation[] Establishments { get; private set; }

        public FindMeBeerApiDriver(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateDefaultClient(new Uri("http://localhost"));
        }

        public async Task<bool> ImportData()
        {
            var importData = await _client.PatchAsync("/data-management/import", null);
            return importData.IsSuccessStatusCode;
        }

        public async Task<bool> GetBeerEstablishments()
        {
            var getBeerResponse = await _client.GetAsync("/beer/nearest-locations");
            if (getBeerResponse.IsSuccessStatusCode)
            {
                Establishments = await getBeerResponse.Content.ReadFromJsonAsync<BeerEstablishmentLocation[]>();
            }

            return getBeerResponse.IsSuccessStatusCode;
        }
    }
}
