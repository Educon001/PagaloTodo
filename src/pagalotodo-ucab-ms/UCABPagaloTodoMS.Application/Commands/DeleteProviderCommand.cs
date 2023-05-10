using MediatR;

namespace UCABPagaloTodoMS.Application.Commands;

public class DeleteProviderCommand : IRequest<Guid>
{
    public Guid Request;

    public DeleteProviderCommand(Guid request)
    {
        Request = request;
    }
}