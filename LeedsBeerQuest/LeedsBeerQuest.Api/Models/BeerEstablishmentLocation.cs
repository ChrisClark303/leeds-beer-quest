using LeedsBeerQuest.Data.Models;

namespace LeedsBeerQuest.Api.Models
{
    public class BeerEstablishmentLocation
    {
        public string? Name { get; set; }
        public Location Location { get; set; }
        public double Distance { get; set; }
    }
}
