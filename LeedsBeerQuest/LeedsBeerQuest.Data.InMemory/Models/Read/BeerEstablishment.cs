using LeedsBeerQuest.App.Models.Read;
namespace LeedsBeerQuest.App.Models.Read
{
    public class BeerEstablishment
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public Uri? Url { get; set; }
        public DateTime Date { get; set; }
        public string? Excerpt { get; set; }
        public Uri? Thumbnail { get; set; }
        public Location? Location { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Twitter { get; set; }
        public EstablishmentRatings? Ratings { get; set; }
        public string[]? Tags { get; set; }
    }
}
