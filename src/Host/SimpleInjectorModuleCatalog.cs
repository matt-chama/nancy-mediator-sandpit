namespace Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;
    using SimpleInjector;

    public sealed class SimpleInjectorModuleCatalog : INancyModuleCatalog
    {
        readonly Container container;

        public SimpleInjectorModuleCatalog(Container container)
        {
            this.container = container;
        }

        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return (INancyModule) container.GetInstance(moduleType);
        }

        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return from r in container.GetCurrentRegistrations()
                where typeof(INancyModule).IsAssignableFrom(r.ServiceType)
                select (INancyModule) r.GetInstance();
        }
    }
}
