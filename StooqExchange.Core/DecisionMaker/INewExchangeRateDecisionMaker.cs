using System.Linq;

namespace StooqExchange.Core.DecisionMaker
{
    public interface INewValueDecisionMaker
    {
        bool ShouldRateBeAdd(ExchangeRate exchangeRate, ExchangeRateValue newExchangeRate);
    }
}