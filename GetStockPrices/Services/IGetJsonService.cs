using GetStockPrices.Models;
using System.Threading.Tasks;

namespace GetStockPrices.Services
{
    public interface IGetJsonService
    {
        Task<RootClass> GetJsonFromApi();
    }
}