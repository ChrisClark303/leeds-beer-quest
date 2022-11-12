using LeedsBeerQuest.Api.Models;

namespace LeedsBeerQuest.Api
{
    public interface IBeerEstablishmentDataParser
    {
        BeerEstablishment[] Parse(string data);
    }
}