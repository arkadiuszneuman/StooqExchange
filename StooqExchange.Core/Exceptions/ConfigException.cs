using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StooqExchange.Core.Exceptions
{
    public class ConfigException : Exception
    {
        public ConfigException(string message)
            : base("Config error: " + message)
        {
        }
    }
}
