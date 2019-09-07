namespace Host
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Nancy;
    using Nancy.Extensions;
    using Process;

    // ReSharper disable once UnusedMember.Global - bound dynamically
    public sealed class RequestModule : NancyModule
    {
        static readonly ConcurrentDictionary<Type, MethodInfo> MethodInfos =
            new ConcurrentDictionary<Type, MethodInfo>();

        public RequestModule(IMediator mediator)
        {
            Type requestBaseType = typeof(IRequest<>);

            IEnumerable<Type> requestTypes = typeof(IProcessLivesHere)
                .Assembly
                .GetTypes()
                .Where(x => x.IsAssignableToGenericType(requestBaseType) &&
                            !x.IsAbstract);

            foreach (Type requestType in requestTypes)
            {
                if (!(requestType
                    .GetCustomAttributes(typeof(EndpointDefinitionAttribute))
                    .FirstOrDefault() is EndpointDefinitionAttribute endpoint))
                    continue;

                object theRequest = Activator.CreateInstance(requestType);

                switch (endpoint.Method)
                {
                    case Method.Post:
                        Post(endpoint.Endpoint, DispatchOthers(mediator, theRequest));

                        break;
                    case Method.Put:
                        Put(endpoint.Endpoint, DispatchOthers(mediator, theRequest));

                        break;
                    case Method.Patch:
                        Patch(endpoint.Endpoint, DispatchOthers(mediator, theRequest));

                        break;
                    case Method.Delete:
                        Delete(endpoint.Endpoint, DispatchOthers(mediator, theRequest));

                        break;
                    case Method.Get:
                        Get(endpoint.Endpoint, DispatchGet(mediator, theRequest));

                        break;
                }
            }
        }

        static Func<dynamic, CancellationToken, Task<HttpStatusCode>> DispatchOthers(
            IMediator mediator,
            object theRequest,
            HttpStatusCode resultCode = HttpStatusCode.NoContent)
        {
            return async (o, token) =>
            {
                MethodInfo method = GetMediatorSendMethodInfo(
                    mediator,
                    typeof(CommandResult));

                Task result = (Task) method.Invoke(
                    mediator,
                    new[] {theRequest, token});

                await result.ConfigureAwait(false);

                return resultCode;
            };
        }

        static Func<dynamic, CancellationToken, Task<object>> DispatchGet(
            IMediator mediator,
            object theRequest)
        {
            return async (o, token) =>
            {
                Type returnType = theRequest
                    .GetType()
                    .GetInterfaces()
                    .First(x => x.GetGenericTypeDefinition() == typeof(IRequest<>))
                    .GetGenericArguments()
                    .First();

                MethodInfo method = GetMediatorSendMethodInfo(
                    mediator,
                    returnType);

                Task result = (Task) method.Invoke(
                    mediator,
                    new[] {theRequest, token});

                await result.ConfigureAwait(false);

                return (object) ((dynamic) result).Result;
            };
        }

        static MethodInfo GetMediatorSendMethodInfo(
            IMediator mediator,
            Type returnType)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            return MethodInfos.GetOrAdd(
                returnType,
                // ReSharper disable once PossibleNullReferenceException
                type => mediator
                    .GetType()
                    .GetMethod(
                        nameof(IMediator.Send),
                        BindingFlags.Instance | BindingFlags.Public)
                    .MakeGenericMethod(type));
        }
    }
}
