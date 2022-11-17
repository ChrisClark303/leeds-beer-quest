using LeedsBeerQuest.App.Models;

namespace LeedsBeerQuest.App
{
    public interface IBeerEstablishmentDataParser
    {
        BeerEstablishment[] Parse(string data);
    }
}