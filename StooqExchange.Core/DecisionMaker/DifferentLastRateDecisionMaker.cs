using System;
using System.Linq;
using StooqExchange.Core.Exceptions;

namespace StooqExchange.Core.DecisionMaker
{
    public class DifferentLastRateDecisionMaker : IDecisionMaker
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
                throw new InvalidExchangeRateException("Data pobrania nowego indeksu gie�dowego nieprawid�owa");

            return exchangeRateValue.Value != newExchangeRate.Value;
        }
    }
}