using LeedsBeerQuest.Api.Models;
using LeedsBeerQuest.Data.Models;

namespace LeedsBeerQuest.Api.Services
{
    public interface IFindMeBeerService
    {
        Task<BeerEstablishmentLocation[]> GetNearestBeerLocations(Location myLocation);
    }

    public interface ILocationDistanceCalculator
    {
        double CalculateDistanceInMiles(Location startLocation, Location targetLocation);
    }

    /// <summary>
    /// The solution here is very heavily based on https://www.geeksforgeeks.org/program-distance-two-points-earth/
    /// </summary>
    public class LocationDistanceCalculator : ILocationDistanceCalculator
    {
        private const int _radiusOfEarthInMiles = 3956;

        public double CalculateDistanceInMiles(Location startLocation, Location targetLocation)
        {
            // The math module contains
            // a function named toRadians
            // which converts from degrees
            // to radians.
            var radLon1 = toRadians(startLocation.Long);
            var radLat1 = toRadians(startLocation.Lat);

            var radLon2 = toRadians(targetLocation.Long);
            var radLat2 = toRadians(targetLocation.Lat);

            // Haversine formula
            double dlon = radLon2 - radLon1;
            double dlat = radLat2 - radLat1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(radLat1) * Math.Cos(radLat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return Math.Round(c * _radiusOfEarthInMiles, 3);
        }

        static double toRadians(double angleIn10thofaDegree)
        {
            // Angle in 10th of a degree
            return angleIn10thofaDegree * Math.PI / 180;
        }
    }
}