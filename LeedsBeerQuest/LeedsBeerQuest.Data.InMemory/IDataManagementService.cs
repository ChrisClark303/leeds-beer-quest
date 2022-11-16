using LeedsBeerQuest.Data.Models;

namespace LeedsBeerQuest.Data
{
    public interface IDataManagementService
    {
        void ImportData(BeerEstablishment[] establishments);
    }
}