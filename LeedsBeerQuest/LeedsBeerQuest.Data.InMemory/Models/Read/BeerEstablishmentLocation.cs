using LeedsBeerQuest.App.Models.Write;

namespace LeedsBeerQuest.App.Models.Read
{
    public class BeerEstablishmentLocation
    {
        private const double _metreToMileConversion = 1609.344;

        public string? Name { get; set; }
        public Location? Location { get; set; }
        public double Distance { get; set; }

        private double _distanceInMetres;
        public double DistanceInMetres 
        {
            get { return _distanceInMetres; }
            set { 
                _distanceInMetres = value; 
                var distanceInMiles = _distanceInMetres / _metreToMileConversion;
                Distance = Math.Round(distanceInMiles, 4);
            } 
        }
    }
}
