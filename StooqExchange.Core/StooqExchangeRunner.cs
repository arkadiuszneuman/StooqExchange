using System;
using System.Collections.Generic;
using System.Linq;
using StooqExchange.Core.ExchangeRateFinder;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using StooqExchange.Core.ConfigManager;
using StooqExchange.Core.DecisionMaker;
using StooqExchange.Core.Exceptions;
using StooqExchange.Core.ExchangeRateArchiveManager;
using StooqExchange.Core.Logger;

namespace StooqExchange.Core
{
    public class StooqExchangeRunner
    {
        private readonly IExchangeFinder exchangeFinder;
        private readonly IExchangeRateArchiveManager archiveManager;
        private readonly INewValueDecisionMaker decisionMaker;
        private readonly IConfigManager configManager;
        private readonly IStooqLogger logger;

        private readonly object syncObject = new object();
        private bool isActionExecuting;
        private bool stopExecuting;

        public StooqExchangeRunner(IExchangeFinder exchangeFinder, IExchangeRateArchiveManager archiveManager,
            INewValueDecisionMaker decisionMaker, IConfigManager configManager, IStooqLogger logger)
        {
            this.exchangeFinder = exchangeFinder;
            this.archiveManager = archiveManager;
            this.decisionMaker = decisionMaker;
            this.configManager = configManager;
            this.logger = logger;
        }

        public async void RunOnce(params string[] stockIndices)
        {
            try
            {
                lock (syncObject)
                {
                    if (isActionExecuting)
                        return;

                    isActionExecuting = true;
                }

                List<ExchangeRate> exchangeRates = archiveManager.Get().ToList();
                bool saveValues = false;

                foreach (string stockIndex in stockIndices)
                {
                    try
                    {
                        ExchangeRateValue exchangeRateValue = await exchangeFinder.FindExchangeAsync(stockIndex);
                        ExchangeRate existingExchangeRate = exchangeRates.SingleOrDefault(e => e.Name == stockIndex);
                        if (existingExchangeRate == null)
                        {
                            exchangeRates.Add(new ExchangeRate(stockIndex, new List<ExchangeRateValue>(new[] {exchangeRateValue})));
                            saveValues = true;
                        }
                        else if (decisionMaker.ShouldRateBeAdd(existingExchangeRate, exchangeRateValue))
                        {
                            existingExchangeRate.Values.Add(exchangeRateValue);
                            saveValues = true;
                        }
                    }
                    catch (Exception e) when (e is ExchangeRateFindException || e is InvalidExchangeRateException)
                    {
                        logger.Warning(e);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        stopExecuting = true;
                    }
                }

                if (saveValues)
                    archiveManager.Save(exchangeRates);

                lock (syncObject)
                    isActionExecuting = false;
            }
            catch (Exception e)
            {
                logger.Error(e);
                stopExecuting = true;
            }
        }

        public void RunInfinite(params string[] stockIndices)
        {
            if (configManager.Get().Interval < 10)
                throw new ConfigException("Interval cannot be lower then 10 seconds");

            RepeatAction(() => RunOnce(stockIndices), TimeSpan.FromSeconds(configManager.Get().Interval));
        }

        private async void RepeatAction(Action action, TimeSpan interval)
        {
            while (true)
            {
                if (!stopExecuting)
                {
                    action();
                    Task task = Task.Delay(interval);
                    await task;
                }
            }
        }
    }
}
