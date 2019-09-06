using Nancy;
using SimpleInjector.Lifestyles;

namespace Host
{
    public sealed class SimpleInjectorScopedContextFactory : INancyContextFactory
    {
        private readonly SimpleInjector.Container _container;
        private readonly INancyContextFactory _defaultFactory;

        public SimpleInjectorScopedContextFactory(
            SimpleInjector.Container container,
            INancyContextFactory @default)
        {
            _container = container;
            _defaultFactory = @default;
        }

        public NancyContext Create(Request request)
        {
            var context = _defaultFactory.Create(request);

            context.Items.Add(
                "SimpleInjector.Scope",
                AsyncScopedLifestyle.BeginScope(_container));

            return context;
        }
    }
}
