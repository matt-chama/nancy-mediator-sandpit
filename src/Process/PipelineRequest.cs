using MediatR;

namespace Process
{
    public abstract class PipelineRequest : IRequest<CommandResult>
    {
    }
}