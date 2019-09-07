namespace Process
{
    using MediatR;

    public abstract class CommandBase : IRequest<CommandResult>
    {
    }
}
