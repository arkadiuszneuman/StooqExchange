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
        public void Save(IEnumerable<ExchangeRate> exchangeRates)
        {
            string json = JsonConvert.SerializeObject(exchangeRates);
            File.WriteAllText(Path, json);
        }

        public IEnumerable<ExchangeRate> Load()
        {
            if (!File.Exists(Path))
                return Enumerable.Empty<ExchangeRate>();

            string json = File.ReadAllText(Path);
            return JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(json);
        }

        public string Path { get; set; } = System.IO.Path.Combine(Assembly.GetEntryAssembly().Location, "exchange-rates.json");
    }
}
