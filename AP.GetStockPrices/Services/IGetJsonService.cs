using System.Threading.Tasks;

namespace AP.GetStockPrices.Services
{
    public interface IGetJsonService
    {
        Task<RootClass> GetJsonFromApi();
    }
}