using Autofac;
using StooqExchange.Core;
using StooqExchange.Core.ExchangeRateArchiveManager;

namespace StooqExchange.Infrastructure.Modules
{
    public class ArchiveManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<JSONExchangeRateArchiveManager>()
                .SingleInstance()
                .As<IExchangeRateArchiveManager>();
        }
    }
}