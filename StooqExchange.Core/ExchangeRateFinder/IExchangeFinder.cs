namespace StooqExchange.Core.ExchangeRateFinder
{
    public interface IExchangeFinder
    {
        decimal FindExchange(string text);
    }
}