using LeedsBeerQuest.Data.Models;

namespace LeedsBeerQuest.Api
{
    public interface IBeerEstablishmentDataParser
    {
        BeerEstablishment[] Parse(string data);
    }
}