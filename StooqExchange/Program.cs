using StooqExchange.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using StooqExchange.Core;

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

                StooqExchangeRunner exchangeRunner = lifetimeScope.Resolve<StooqExchangeRunner>();
                exchangeRunner.RunInfinite("WIG", "WIG20");

                while (true)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                    if (consoleKeyInfo.Key == ConsoleKey.Q)
                        break;
                }
            }
        }
    }
}
