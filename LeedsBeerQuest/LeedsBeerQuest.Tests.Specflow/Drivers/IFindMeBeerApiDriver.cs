using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.Tests.Specflow.Drivers
{
    internal interface IFindMeBeerApiDriver
    {
        BeerEstablishmentLocation[] Establishments { get; }

        Task<bool> GetBeerEstablishments();
        Task<bool> ImportData();
    }
}