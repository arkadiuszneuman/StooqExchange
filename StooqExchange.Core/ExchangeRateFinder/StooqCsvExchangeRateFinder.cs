using StooqExchange.Core.Exceptions;
using System;
using System.Threading.Tasks;
using StooqExchange.Core.HttpDownloader;
using StooqExchange.Core.Logger;

namespace StooqExchange.Core.ExchangeRateFinder
{
    public class StooqCsvExchangeRateFinder : IExchangeFinder
    {
        private readonly IHttpDownloader httpDownloader;
        private readonly IDateTimeGetter dateTimeGetter;
        private readonly IStooqLogger logger;

        public StooqCsvExchangeRateFinder(IHttpDownloader httpDownloader, IDateTimeGetter dateTimeGetter, IStooqLogger logger)
        {
            this.httpDownloader = httpDownloader;
            this.dateTimeGetter = dateTimeGetter;
            this.logger = logger;
        }

        public async Task<ExchangeRateValue> FindExchangeAsync(string stockIndex)
        {
            logger.Info(string.Format("Finding {0} value", stockIndex));
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

            logger.Info($"Value of index {stockIndex} equals {exchangeResult}");
            return new ExchangeRateValue(dateTimeGetter.GetDateTime(), exchangeResult);
        }

        private void ThrowInvalidCsvData()
        {
            throw new ExchangeRateFindException("Invalid CSV data");
        }
    }
}