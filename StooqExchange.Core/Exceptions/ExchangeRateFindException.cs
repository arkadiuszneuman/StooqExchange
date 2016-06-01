using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StooqExchange.Core.Exceptions
{
    public class ExchangeRateFindException : Exception
    {
        public ExchangeRateFindException(string message)
            : base(message)
        {
        }
    }
}
