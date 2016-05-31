using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace StooqExchange.Infrastructure
{
    public class StooqContainer
    {
        public IContainer CreateContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(GetType().GetTypeInfo().Assembly);

            return builder.Build();
        }
    }
}
