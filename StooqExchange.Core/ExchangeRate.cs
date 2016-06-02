using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StooqExchange.Core
{
    public class ExchangeRate
    {
        public ExchangeRate()
        {
        }

        public ExchangeRate(string name, List<ExchangeRateValue> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; set; }
        public List<ExchangeRateValue> Values { get; set; }
    }
}
