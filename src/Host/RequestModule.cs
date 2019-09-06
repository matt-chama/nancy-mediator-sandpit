using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Nancy;
using Nancy.Extensions;
using Process;

namespace Host
{
    public sealed class RequestModule : NancyModule
    {
        public RequestModule(IMediator mediator)
        {
            var requestBaseType = typeof(IRequest<>);

            var requestTypes = typeof(IProcessLivesHere)
                .Assembly
                .GetTypes()
                .Where(x => x.IsAssignableToGenericType(requestBaseType) &&
                            !x.IsAbstract);

            foreach (var requestType in requestTypes)
            {
                if (!(requestType
                    .GetCustomAttributes(typeof(EndpointDefinitionAttribute))
                    .FirstOrDefault() is EndpointDefinitionAttribute endpoint))
                {
                    continue;
                }

                object theRequest = Activator.CreateInstance(requestType);

                switch (endpoint.Method)
                {
                    case Method.Post:
                        Post(endpoint.Endpoint, (o, token) =>
                        {
                            MethodInfo method = GetMeditorSendMethodInfo(
                                mediator,
                                typeof(CommandResult));

                            method.Invoke(mediator, new[] {theRequest, token});

                            return Task.FromResult(HttpStatusCode.Accepted);
                        });
                        break;
                    case Method.Get:
                        Get(endpoint.Endpoint, async (o, token) =>
                        {
                            Type returnType = theRequest
                                .GetType()
                                .GetInterfaces()
                                .First()
                                .GetGenericArguments()
                                .First();

                            var method = GetMeditorSendMethodInfo(
                                mediator,
                                returnType);

                            Task result = (Task)method.Invoke(
                                mediator,
                                new[] { theRequest, token });

                            await result.ConfigureAwait(false);

                            return (object)((dynamic) result).Result;
                        });
                        break;
                }
            }
        }

        static MethodInfo GetMeditorSendMethodInfo(
            IMediator mediator,
            Type returnType)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            // ReSharper disable once PossibleNullReferenceException
            return mediator
                .GetType()
                .GetMethod(
                    "Send",
                    BindingFlags.Instance | BindingFlags.Public)
                .MakeGenericMethod(returnType);
        }
    }
}
