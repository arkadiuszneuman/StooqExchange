using StooqExchange.Core.Exceptions;
using System;

namespace StooqExchange.Core.ExchangeRateFinder
{
    public class StooqCsvExchangeRateFinder : IExchangeFinder
    {
        public decimal FindExchange(string csv)
        {
            var splittedData = csv.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedData.Length != 2)
                ThrowInvalidCsvData();

            var exchanges = splittedData[1].Split(',');

            if (exchanges.Length != 8)
                ThrowInvalidCsvData();

            var exchangeAsString = exchanges[6].Replace('.', ',');

            decimal exchangeResult;
            if (!decimal.TryParse(exchangeAsString, out exchangeResult))
                ThrowInvalidCsvData();

            return exchangeResult;
        }

        private void ThrowInvalidCsvData()
        {
            throw new ExchangeRateFindException("Nieprawid³owe dane csv");
        }
    }
}