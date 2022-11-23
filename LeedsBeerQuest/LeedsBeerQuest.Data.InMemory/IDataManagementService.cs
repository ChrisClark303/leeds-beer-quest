using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App
{
    public interface IDataManagementService
    {
        Task ImportData(BeerEstablishment[] establishments);
    }
}