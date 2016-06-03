using System;
using System.Collections.Generic;
using System.Linq;
using StooqExchange.Core.ExchangeRateFinder;
using System.Threading;
using System.Threading.Tasks;
using StooqExchange.Core.DecisionMaker;
using StooqExchange.Core.ExchangeRateSaver;

namespace StooqExchange.Core
{
    public class StooqExchangeRunner
    {
        private readonly IExchangeFinder exchangeFinder;
        private readonly IExchangeRateFileManager fileManager;
        private readonly INewExchangeRateDecisionMaker decisionMaker;
        private readonly object syncObject = new object();
        private bool isActionExecuting;

        public StooqExchangeRunner(IExchangeFinder exchangeFinder, IExchangeRateFileManager fileManager,
            INewExchangeRateDecisionMaker decisionMaker)
        {
            this.exchangeFinder = exchangeFinder;
            this.fileManager = fileManager;
            this.decisionMaker = decisionMaker;
        }

        public async void RunOnce(params string[] stockIndices)
        {
            lock (syncObject)
            {
                if (isActionExecuting)
                    return;

                isActionExecuting = true;
            }

            List<ExchangeRate> exchangeRates = fileManager.Get().ToList();
            bool saveValues = false;

            foreach (string stockIndex in stockIndices)
            {
                ExchangeRateValue exchangeRateValue = await exchangeFinder.FindExchangeAsync(stockIndex);
                ExchangeRate existingExchangeRate = exchangeRates.SingleOrDefault(e => e.Name == stockIndex);
                if (existingExchangeRate == null)
                {
                    exchangeRates.Add(new ExchangeRate(stockIndex, new List<ExchangeRateValue>(new[] { exchangeRateValue })));
                    saveValues = true;
                }
                else if (decisionMaker.ShouldRateBeAdd(existingExchangeRate, exchangeRateValue))
                {
                    existingExchangeRate.Values.Add(exchangeRateValue);
                    saveValues = true;
                }
            }

            if (saveValues)
                fileManager.Save(exchangeRates);

            lock (syncObject)
                isActionExecuting = false;
        }

        public void RunInfinite(params string[] stockIndices)
        {
            RepeatAction(() => RunOnce(stockIndices), TimeSpan.FromSeconds(10));
        }

        private async void RepeatAction(Action action, TimeSpan interval)
        {
            while (true)
            {
                action();
                Task task = Task.Delay(interval);
                await task;
            }
        }
    }
}
