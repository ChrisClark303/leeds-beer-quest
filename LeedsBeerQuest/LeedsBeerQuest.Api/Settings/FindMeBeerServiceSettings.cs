using LeedsBeerQuest.Data.Models;

namespace LeedsBeerQuest.Api.Settings
{
    public class FindMeBeerServiceSettings
    {
        public int DefaultPageSize { get; set; }
        public Location DefaultSearchLocation { get; set; }
    }
}
