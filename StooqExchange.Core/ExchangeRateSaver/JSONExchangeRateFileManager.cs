using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StooqExchange.Core.ExchangeRateSaver
{
    public class JSONExchangeRateFileManager : IExchangeRateFileManager
    {
        private ICollection<ExchangeRate> loadedExchangeRates;

        public virtual void Save(IEnumerable<ExchangeRate> exchangeRates)
        {
            string json = JsonConvert.SerializeObject(exchangeRates);
            File.WriteAllText(Path, json);

            loadedExchangeRates = exchangeRates.ToList();
        }

        public virtual IEnumerable<ExchangeRate> Load()
        {
            if (!File.Exists(Path))
                return Enumerable.Empty<ExchangeRate>();

            string json = File.ReadAllText(Path);
            var exchangeRates = JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(json);

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
