using StooqExchange.Core.Exceptions;
using System;
using System.Threading.Tasks;
using StooqExchange.Core.HttpDownloader;

namespace StooqExchange.Core.ExchangeRateFinder
{
    public class StooqCsvExchangeRateFinder : IExchangeFinder
    {
        private readonly IHttpDownloader httpDownloader;
        private readonly IDateTimeGetter dateTimeGetter;

        public StooqCsvExchangeRateFinder(IHttpDownloader httpDownloader, IDateTimeGetter dateTimeGetter)
        {
            this.httpDownloader = httpDownloader;
            this.dateTimeGetter = dateTimeGetter;
        }

        public async Task<ExchangeRateValue> FindExchangeAsync(string stockIndex)
        {
            string csv = await httpDownloader.DownloadAsync(stockIndex);

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

            return new ExchangeRateValue(dateTimeGetter.GetDateTime(), exchangeResult);
        }

        private void ThrowInvalidCsvData()
        {
            throw new ExchangeRateFindException("Nieprawid³owe dane csv");
        }
    }
}