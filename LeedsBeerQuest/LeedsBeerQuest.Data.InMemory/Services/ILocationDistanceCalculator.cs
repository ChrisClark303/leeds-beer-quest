using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App.Services
{
    public interface ILocationDistanceCalculator
    {
        double CalculateDistanceInMetres(Location startLocation, Location targetLocation);
    }
}