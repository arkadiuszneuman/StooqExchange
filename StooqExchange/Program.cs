using StooqExchange.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using StooqExchange.Core;
using StooqExchange.Core.ConfigManager;
using StooqExchange.Core.Logger;

namespace StooqExchange
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Press Q to quit");

            IContainer container = new StooqContainer().CreateContainer();
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var configManager = lifetimeScope.Resolve<IConfigManager>();
                var logger = lifetimeScope.Resolve<IStooqLogger>();
                var exchangeRunner = lifetimeScope.Resolve<StooqExchangeRunner>();

                try
                {
                    Config config = configManager.Get();
                    exchangeRunner.RunInfinite(config.StockIndices);

                    while (true)
                    {
                        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                        if (consoleKeyInfo.Key == ConsoleKey.Q)
                            break;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }
    }
}
