using Autofac;
using StooqExchange.Core.ConfigManager;
using StooqExchange.Core.Logger;

namespace StooqExchange.Infrastructure.Modules
{
    public class ConfigManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<JSONConfigManager>()
                .SingleInstance()
                .As<IConfigManager>();
        }
    }
}