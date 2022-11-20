using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App.Services
{
    public interface ILocationDistanceCalculator
    {
        double CalculateDistanceInMiles(Location startLocation, Location targetLocation);
    }
}