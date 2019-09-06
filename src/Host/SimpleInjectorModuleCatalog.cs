using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Host
{
    public sealed class SimpleInjectorModuleCatalog : INancyModuleCatalog
    {
        private readonly SimpleInjector.Container _container;

        public SimpleInjectorModuleCatalog(SimpleInjector.Container container)
        {
            this._container = container;
        }

        public INancyModule GetModule(Type moduleType, NancyContext context) =>
            (INancyModule)_container.GetInstance(moduleType);

        public IEnumerable<INancyModule> GetAllModules(NancyContext context) =>
            from r in _container.GetCurrentRegistrations()
            where typeof(INancyModule).IsAssignableFrom(r.ServiceType)
            select (INancyModule)r.GetInstance();
    }
}
