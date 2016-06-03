using System.Linq;

namespace StooqExchange.Core.DecisionMaker
{
    public interface INewExchangeRateDecisionMaker
    {
        bool ShouldRateBeAdd(ExchangeRate exchangeRate, ExchangeRateValue newExchangeRate);
    }
}