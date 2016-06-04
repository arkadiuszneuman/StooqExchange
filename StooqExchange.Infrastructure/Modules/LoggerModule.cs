using Autofac;
using StooqExchange.Core.Logger;

namespace StooqExchange.Infrastructure.Modules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<NLogLogger>()
                .SingleInstance()
                .As<IStooqLogger>();
        }
    }
}