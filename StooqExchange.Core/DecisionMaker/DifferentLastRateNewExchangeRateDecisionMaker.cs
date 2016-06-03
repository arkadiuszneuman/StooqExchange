using System;
using System.Linq;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.Core.DecisionMaker
{
    public class DifferentLastRateNewExchangeRateDecisionMaker : INewExchangeRateDecisionMaker
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