using System.Collections.Generic;

namespace StooqExchange.Core.ExchangeRateArchiveManager
{
    public interface IExchangeRateArchiveManager
    {
        void Save(IEnumerable<ExchangeRate> exchangeRate);
        IEnumerable<ExchangeRate> Load();
        IEnumerable<ExchangeRate> Get();
    }
}
