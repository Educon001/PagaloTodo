using MediatR;

namespace UCABPagaloTodoMS.Application.Commands;

public class DeleteConsumerCommand : IRequest<Guid>
{
    public Guid Request;

    public DeleteConsumerCommand(Guid request)
    {
        Request = request;
    }
}