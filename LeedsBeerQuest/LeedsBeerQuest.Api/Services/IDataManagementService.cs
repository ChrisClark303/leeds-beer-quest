using LeedsBeerQuest.Api.Models;

namespace LeedsBeerQuest.Api.Services
{
    public interface IDataManagementService
    {
        void ImportData(BeerEstablishment[] establishments);
    }
}