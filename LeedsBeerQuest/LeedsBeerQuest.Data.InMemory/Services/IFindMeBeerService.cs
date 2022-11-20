using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App.Services
{
    public interface IFindMeBeerService
    {
        Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location? myLocation = null);
    }
}