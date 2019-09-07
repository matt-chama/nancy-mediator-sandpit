using Nancy;
using SimpleInjector.Lifestyles;

namespace Host
{
    public sealed class SimpleInjectorScopedContextFactory : INancyContextFactory
    {
        readonly SimpleInjector.Container container;
        readonly INancyContextFactory defaultFactory;

        public SimpleInjectorScopedContextFactory(
            SimpleInjector.Container container,
            INancyContextFactory @default)
        {
            this.container = container;
            defaultFactory = @default;
        }

        public NancyContext Create(Request request)
        {
            var context = defaultFactory.Create(request);

            context.Items.Add(
                "SimpleInjector.Scope",
                AsyncScopedLifestyle.BeginScope(container));

            return context;
        }
    }
}
