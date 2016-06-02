using System.Linq;

namespace StooqExchange.Core.DecisionMaker
{
    public interface IDecisionMaker
    {
        bool ShouldRateBeAdd(ExchangeRate exchangeRate, ExchangeRateValue newExchangeRate);
    }
}