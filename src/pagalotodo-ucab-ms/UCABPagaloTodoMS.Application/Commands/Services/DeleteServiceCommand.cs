using MediatR;

namespace UCABPagaloTodoMS.Application.Commands.Services;

public class DeleteServiceCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteServiceCommand(Guid id)
    {
        Id = id;
    }
}