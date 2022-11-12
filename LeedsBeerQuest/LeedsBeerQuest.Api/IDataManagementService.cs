using LeedsBeerQuest.Api.Models;

namespace LeedsBeerQuest.Api
{
    public interface IDataManagementService
    {
        void ImportData(BeerEstablishment[] establishments);
    }
}