using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Api.Models
{
    public class BeerEstablishment
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public Uri Url { get; set; }
        public DateTime Date { get; set; }
        public string Excerpt { get; set; }         
        public Uri Thumbnail { get; set; }
        public Location Location { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public EstablishmentRatings Ratings { get; set; }
        public string[] Tags { get; set; }
    }

    public struct EstablishmentRatings
    {
        public decimal Beer { get; set; }
        public decimal Atmosphere { get; set; }
        public decimal Amenities { get; set; }
        public decimal Value { get; set; }
    }

    public struct Location
    {
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
    }
}
