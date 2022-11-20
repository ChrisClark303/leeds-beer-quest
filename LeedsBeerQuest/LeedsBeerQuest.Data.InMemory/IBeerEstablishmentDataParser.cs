using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App
{
    public interface IBeerEstablishmentDataParser
    {
        BeerEstablishment[] Parse(string data);
    }
}