using System;

namespace StooqExchange.Core.Exceptions
{
    public class InvalidExchangeRateException : Exception
    {
        public InvalidExchangeRateException(string message)
            : base(message)
        {
        }
    }
}