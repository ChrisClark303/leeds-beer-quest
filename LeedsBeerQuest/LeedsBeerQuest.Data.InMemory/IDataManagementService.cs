using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App
{
    public interface IDataManagementService
    {
        void ImportData(BeerEstablishment[] establishments);
    }
}