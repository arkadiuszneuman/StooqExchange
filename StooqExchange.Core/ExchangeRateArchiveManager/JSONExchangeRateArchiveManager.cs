using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using StooqExchange.Core.Logger;

namespace StooqExchange.Core.ExchangeRateArchiveManager
{
    /// <summary>
    /// Class is responsible for save and load rate archive to/from JSON file. 
    /// By default file is saved to application directory as exchange-rates.json.
    /// </summary>
    public class JSONExchangeRateArchiveManager : IExchangeRateArchiveManager
    {
        private readonly IStooqLogger logger;
        private ICollection<ExchangeRate> loadedExchangeRates;

        public JSONExchangeRateArchiveManager(IStooqLogger logger)
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
                return loadedExchangeRates = Enumerable.Empty<ExchangeRate>().ToList();

            logger.Info("Loading data archive");
            string json = File.ReadAllText(Path);
            var exchangeRates = JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(json);

            logger.Info("Data archive loaded");

            return loadedExchangeRates = exchangeRates.ToList() ?? Enumerable.Empty<ExchangeRate>().ToList();
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
