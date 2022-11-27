using LeedsBeerQuest.App.Models.Write;

namespace LeedsBeerQuest.App.Models.Read
{
    public class BeerEstablishmentLocation
    {
        public string? Name { get; set; }
        public Location? Location { get; set; }

        public double DistanceInMetres { get; set; }
    }
}
