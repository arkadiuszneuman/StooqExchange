using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using StooqExchange.Core;
using Module = Autofac.Module;

namespace StooqExchange.Infrastructure.Modules
{
    public class ImplementedInterfacesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(StooqExchangeRunner).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
        }
    }
}
