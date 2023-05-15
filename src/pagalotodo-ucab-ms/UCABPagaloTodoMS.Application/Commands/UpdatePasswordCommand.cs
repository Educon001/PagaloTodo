using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands.Services;


public class UpdatePasswordCommand : IRequest<UpdatePasswordResponse>
{
    public UpdatePasswordRequest Request { get; set; }
    public Guid Id { get; set; }

    public UpdatePasswordCommand(UpdatePasswordRequest request, Guid id)
    {
        Request = request;
        Id = id;
    }
}