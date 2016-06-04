using System.Collections.Generic;

namespace StooqExchange.Core.ConfigManager
{
    public class Config
    {
        public int Interval { get; set; } = 60;
        public string[] StockIndices { get; set; } = new[] {"WIG", "WIG20", "FW20", "mWIG40", "sWIG80" };
    }
}