using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StooqExchange.Core.Logger;

namespace StooqExchange.Core.ExchangeRateSaver
{
    public class JSONExchangeRateFileManager : IExchangeRateFileManager
    {
        private readonly IStooqLogger logger;
        private ICollection<ExchangeRate> loadedExchangeRates;

        public JSONExchangeRateFileManager(IStooqLogger logger)
        {
            this.logger = logger;
        }

        public virtual void Save(IEnumerable<ExchangeRate> exchangeRates)
        {
            logger.Info("Saving data archive");
            string json = JsonConvert.SerializeObject(exchangeRates);
            File.WriteAllText(Path, json);

            loadedExchangeRates = exchangeRates.ToList();

            logger.Info("Data archive saved");
        }

        public virtual IEnumerable<ExchangeRate> Load()
        {
            if (!File.Exists(Path))
                return Enumerable.Empty<ExchangeRate>();

            logger.Info("Loading data archive");
            string json = File.ReadAllText(Path);
            var exchangeRates = JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(json);

            logger.Info("Data archive loaded");

            return exchangeRates ?? Enumerable.Empty<ExchangeRate>();
        }

        public virtual IEnumerable<ExchangeRate> Get()
        {
            if (loadedExchangeRates == null)
                loadedExchangeRates = Load().ToList();

            return loadedExchangeRates;
        }

        public string Path { get; set; } = System.IO.Path.Combine(System.IO.Path
            .GetDirectoryName(Assembly.GetEntryAssembly().Location), "exchange-rates.json");
    }
}
