using System;

namespace StooqExchange.Core.ExchangeRateFinder
{
    public class CurrentDateTimeGetter : IDateTimeGetter
    {
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }
    }
}