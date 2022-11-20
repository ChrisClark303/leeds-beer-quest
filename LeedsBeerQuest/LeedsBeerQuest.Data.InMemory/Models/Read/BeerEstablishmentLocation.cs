using LeedsBeerQuest.App.Models.Write;

namespace LeedsBeerQuest.App.Models.Read
{
    //TODO: There's a serialisation issue here where If DistanceInMetres gets set AFTER
    //Distance, then it overwrites it. This is partially resolved by moving Distance underneath
    //DistanceInMetres, but that's a ridiculous solution.
    public class BeerEstablishmentLocation
    {
        private const double _metreToMileConversion = 1609.344;

        public string? Name { get; set; }
        public Location? Location { get; set; }

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

        public double Distance { get; set; }
    }
}
