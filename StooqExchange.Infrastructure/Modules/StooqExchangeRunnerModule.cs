using Autofac;
using StooqExchange.Core;

namespace StooqExchange.Infrastructure.Modules
{
    public class StooqExchangeRunnerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<StooqExchangeRunner>()
                .SingleInstance()
                .AsSelf();
        }
    }
}