using LeedsBeerQuest.App.Models.Read;
using System.Net;

namespace LeedsBeerQuest.Tests.Specflow.Drivers
{
    internal interface IFindMeBeerApiDriver
    {
        BeerEstablishmentLocation[] Establishments { get; }
        BeerEstablishment? EstablishmentByName { get; }
        HttpStatusCode LastRequestStatusCode { get; }

        void SetSearchLocation(Location? location = null);
        Task<bool> GetBeerEstablishments();
        Task<bool> ImportData();
        void SetSearchEstablishmentName(string establishmentName);
        Task<bool> GetBeerEstablishmentByName();
    }
}