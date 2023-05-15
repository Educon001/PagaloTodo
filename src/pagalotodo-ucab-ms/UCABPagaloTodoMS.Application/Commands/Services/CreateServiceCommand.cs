using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands.Services;

public class CreateServiceCommand : IRequest<Guid>
{
    public ServiceRequest Request { get; set; }

    public CreateServiceCommand(ServiceRequest request)
    {
        Request = request;
    }
}