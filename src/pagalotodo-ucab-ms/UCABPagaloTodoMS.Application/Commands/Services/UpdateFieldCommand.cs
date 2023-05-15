using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands.Services;

public class UpdateFieldCommand : IRequest<FieldResponse>
{
    public FieldRequest Request { get; set; }

    public Guid Id { get; set; }
    
    public UpdateFieldCommand(FieldRequest request, Guid id)
    {
        Request = request;
        Id = id;
    }
}