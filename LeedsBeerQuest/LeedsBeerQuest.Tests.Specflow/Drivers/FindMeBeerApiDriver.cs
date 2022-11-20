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
        private Location? _location;

        public BeerEstablishmentLocation[]? Establishments { get; private set; }

        public FindMeBeerApiDriver(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateDefaultClient(new Uri("http://localhost"));
        }

        public async Task<bool> ImportData()
        {
            var importData = await _client.PatchAsync("/data-management/import", null);
            return importData.IsSuccessStatusCode;
        }

        public void SetSearchLocation(Location? location = null)
        {
            _location = location;
        }

        public async Task<bool> GetBeerEstablishments()
        {
            var searchUrl = "/beer/nearest-locations";
            if (_location != null)
            {
                searchUrl = $"{searchUrl}?lat={_location.Lat}&lng={_location.Long}";
            }
            var getBeerResponse = await _client.GetAsync(searchUrl);
            if (getBeerResponse.IsSuccessStatusCode)
            {
                Establishments = await getBeerResponse.Content.ReadFromJsonAsync<BeerEstablishmentLocation[]>();
            }

            return getBeerResponse.IsSuccessStatusCode;
        }
    }
}
