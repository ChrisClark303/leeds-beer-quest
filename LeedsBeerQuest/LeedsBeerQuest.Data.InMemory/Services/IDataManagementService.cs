using LeedsBeerQuest.App.Models.Read;

namespace LeedsBeerQuest.App.Services
{
    public interface IDataManagementService
    {
        Task ImportData(BeerEstablishment[] establishments);
    }
}