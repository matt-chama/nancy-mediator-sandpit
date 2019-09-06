using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
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
                .Where(x => x.IsAssignableToGenericType(requestBaseType) && !x.IsAbstract);

            foreach (var requestType in requestTypes)
            {
                EndpointDefinitionAttribute endpoint = requestType
                    .GetCustomAttributes(typeof(EndpointDefinitionAttribute))
                    .FirstOrDefault() as EndpointDefinitionAttribute;

                if (endpoint == null)
                {
                    continue;
                }

                if (endpoint.Method == "POST")
                {
                    Post(endpoint.Endpoint, (o, token) =>
                    {
                        object theRequest = Activator.CreateInstance(requestType);
                        this.BindTo(theRequest);

                        // ReSharper disable once PossibleNullReferenceException
                        MethodInfo method = mediator
                            .GetType()
                            .GetMethod(
                                "Send",
                                BindingFlags.Instance | BindingFlags.Public)
                            .MakeGenericMethod(typeof(CommandResult));

                        method.Invoke(mediator, new[] {theRequest, token});

                        return Task.FromResult(HttpStatusCode.Accepted);
                    });
                }
                else if (endpoint.Method == "GET")
                {
                    Get(endpoint.Endpoint, async (o, token) =>
                    {
                        object theRequest = Activator.CreateInstance(requestType);

                        Type returnType = theRequest
                            .GetType()
                            .GetInterfaces()
                            .First()
                            .GetGenericArguments()
                            .First();

                        this.BindTo(theRequest);

                        // ReSharper disable once PossibleNullReferenceException
                        MethodInfo method = mediator
                            .GetType()
                            .GetMethod(
                                "Send",
                                BindingFlags.Instance | BindingFlags.Public)
                            .MakeGenericMethod(returnType);

                        Type returnTask = typeof(Task<>).MakeGenericType(returnType);

                        Task result = (Task)method.Invoke(mediator, new[] {theRequest, token});

                        await result.ConfigureAwait(false);

                        return (object)((dynamic) result).Result;
                    });
                }
            }
        }
    }
}
