using Autofac;
using StooqExchange.Core;
using StooqExchange.Core.ExchangeRateSaver;

namespace StooqExchange.Infrastructure.Modules
{
    public class FileManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<JSONExchangeRateFileManager>()
                .SingleInstance()
                .As<IExchangeRateFileManager>();
        }
    }
}