using System.Collections.Generic;

namespace StooqExchange.Core.ExchangeRateSaver
{
    public interface IExchangeRateFileManager
    {
        void Save(IEnumerable<ExchangeRate> exchangeRate);
    }
}
