using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands.Services;

public class UpdateServiceCommand : IRequest<ServiceResponse>
{
    public ServiceRequest Request { get; set; }

    public Guid Id { get; set; }
    public UpdateServiceCommand(ServiceRequest request, Guid id)
    {
        Request = request;
        Id = id;
    }
}
