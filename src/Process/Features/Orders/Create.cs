namespace Process.Features.Orders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class Create
    {
        [EndpointDefinition(Method.Post, "/orders")]
        public class Command : CommandBase
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
