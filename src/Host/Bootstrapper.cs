namespace Host
{
    using System;
    using System.Reflection;
    using IoC;
    using MediatR;
    using MediatR.Pipeline;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Process;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(
            TinyIoCContainer nancy,
            IPipelines pipelines)
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            Assembly assemblies = typeof(IProcessLivesHere).Assembly;

            // register services
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            // pipeline
            container.Register(() =>
                new ServiceFactory(container.GetInstance), Lifestyle.Singleton);

            container.Collection.Register(
                typeof(IPipelineBehavior<,>),
                new[]
                {
                    typeof(RequestPreProcessorBehavior<,>),
                    typeof(RequestPostProcessorBehavior<,>)
                });
            container.Collection.Register(typeof(IRequestPreProcessor<>), Array.Empty<Type>());
            container.Collection.Register(typeof(IRequestPostProcessor<,>), Array.Empty<Type>());

            // register nancy modules
            foreach (ModuleRegistration nancyModule in Modules)
            {
                container.Register(nancyModule.ModuleType);
            }

            container.Verify();

            // hook up SimpleInjector in the Nancy pipeline
            nancy.Register(
                typeof(INancyModuleCatalog),
                new SimpleInjectorModuleCatalog(container));

            nancy.Register(
                typeof(INancyContextFactory),
                new SimpleInjectorScopedContextFactory(
                    container, nancy.Resolve<INancyContextFactory>()));
        }
    }
}
