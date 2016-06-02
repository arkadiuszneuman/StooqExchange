using System.Threading.Tasks;

namespace StooqExchange.Core.ExchangeRateFinder
{
    public interface IExchangeFinder
    {
        Task<ExchangeRateValue> FindExchangeAsync(string stockIndex);
    }
}