using LeedsBeerQuest.App.Models;

namespace LeedsBeerQuest.App
{
    public interface IDataManagementService
    {
        void ImportData(BeerEstablishment[] establishments);
    }
}