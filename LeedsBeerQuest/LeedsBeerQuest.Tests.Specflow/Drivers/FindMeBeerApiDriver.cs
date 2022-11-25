using LeedsBeerQuest.App.Models.Read;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Specflow.Drivers
{
    internal class FindMeBeerApiDriver : IFindMeBeerApiDriver
    {
        private readonly HttpClient _client;
        private Location? _location;
        private string? _establishmentName;

        public BeerEstablishmentLocation[]? Establishments { get; private set; }
        public BeerEstablishment? EstablishmentByName { get; private set; }
        public HttpStatusCode LastRequestStatusCode { get; private set; }

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

        public void SetSearchEstablishmentName(string establishmentName)
        {
            _establishmentName = establishmentName;
        }

        public async Task<bool> GetBeerEstablishments()
        {
            var searchUrl = "/beer/nearest-establishments";
            if (_location != null)
            {
                searchUrl = $"{searchUrl}?lat={_location.Lat}&lng={_location.Long}";
            }
            return await GetApiResponse<BeerEstablishmentLocation[]>(searchUrl, response =>
            {
                Establishments = response;
            });
        }

        public async Task<bool> GetBeerEstablishmentByName()
        {
            return await GetApiResponse<BeerEstablishment>($"/beer/{_establishmentName}", response =>
            {
                EstablishmentByName = response;
            });
        }

        private async Task<bool> GetApiResponse<TResponseType>(string url, Action<TResponseType?> action)
        {
            var getBeerByNameResponse = await _client.GetAsync(url);
            LastRequestStatusCode = getBeerByNameResponse.StatusCode;
            if (getBeerByNameResponse.IsSuccessStatusCode)
            {
                if (getBeerByNameResponse.Content.Headers.ContentLength > 0)
                {
                    action(await getBeerByNameResponse.Content.ReadFromJsonAsync<TResponseType>());
                }
            }

            return getBeerByNameResponse.IsSuccessStatusCode;
        }
    }
}
