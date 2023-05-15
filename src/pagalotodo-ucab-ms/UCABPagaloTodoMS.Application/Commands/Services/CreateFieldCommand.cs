using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands.Services;

public class CreateFieldCommand : IRequest<Guid>
{
    public FieldRequest Request { get; set; }

    public CreateFieldCommand(FieldRequest request)
    {
        Request = request;
    }
}