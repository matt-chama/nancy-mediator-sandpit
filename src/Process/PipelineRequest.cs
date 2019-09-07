namespace Process
{
    using MediatR;

    public abstract class PipelineRequest : IRequest<CommandResult>
    {
    }
}
