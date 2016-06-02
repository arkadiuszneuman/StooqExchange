using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StooqExchange.Core
{
    public class ExchangeRateValue
    {
        public ExchangeRateValue()
        {
        }

        public ExchangeRateValue(DateTime downloadTime, decimal value)
        {
            DownloadTime = downloadTime;
            Value = value;
        }

        public DateTime DownloadTime { get; set; }
        public decimal Value { get; set; }
    }
}
