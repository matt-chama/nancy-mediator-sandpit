using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Process.Features.Orders
{
    public class Create
    {
        [EndpointDefinition("POST", "/orders")]
        public class Command : PipelineRequest
        {
            public Guid SupplierId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommandResult>
        {
            public Task<CommandResult> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(CommandResult.Void);
            }
        }
    }
}
