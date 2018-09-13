using System;
using System.Text;

using Autofac;
using Autofac.Core;

namespace QK.Framework.Core.Ioc
{
    public class IocContainer
    {
        private static IContainer container;

        public static void RegisterContainer(IContainer container)
        {
            IocContainer.container = container;
        }

        public static TService Resolve<TService>()
        {
            return container.Resolve<TService>();
        }

        public static TService Resolve<TService>(params Parameter[] parameter)
        {
            return container.Resolve<TService>(parameter);
        }

        public static TService ResolveKeyed<TService>(object serviceKey)
        {
            return container.ResolveKeyed<TService>(serviceKey);
        }

        public static TService ResolveNamed<TService>(string serviceName)
        {
            return container.ResolveNamed<TService>(serviceName);
        }
    }
}
