using System;
using System.Linq;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.Core.DecisionMaker
{
    /// <summary>
    /// Class is responsible for decision if new exchange rate should be added to values in exchange rates archive.
    /// Return true if new value is different then previous value.
    /// </summary>
    public class DifferentLastRateNewValueDecisionMaker : INewValueDecisionMaker
    {
        public bool ShouldRateBeAdd(ExchangeRate exchangeRate, ExchangeRateValue newExchangeRate)
        {
            if (exchangeRate == null)
                throw new ArgumentNullException(nameof(exchangeRate));

            if (newExchangeRate == null)
                throw new ArgumentNullException(nameof(newExchangeRate));

            if (!exchangeRate.Values.Any())
                return true;

            ExchangeRateValue exchangeRateValue = exchangeRate.Values.OrderBy(d => d.DownloadTime).Last();
            if (exchangeRateValue.DownloadTime > newExchangeRate.DownloadTime)
                throw new InvalidExchangeRateException("Download time from new stock index is invalid");

            return exchangeRateValue.Value != newExchangeRate.Value;
        }
    }
}