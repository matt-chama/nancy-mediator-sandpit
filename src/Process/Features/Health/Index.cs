using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Process.Features.Health
{
    public class Index
    {
        [EndpointDefinition(Method.Get, "/health")]
        public class Query : IRequest<Model>
        {
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            public Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Model
                {
                    ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });
            }
        }

        public class Model
        {
            public long ServerTime { get; set; }
        }
    }
}