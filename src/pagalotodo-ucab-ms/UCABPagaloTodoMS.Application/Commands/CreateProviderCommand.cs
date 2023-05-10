using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands;

public class CreateProviderCommand : IRequest<Guid>
{
    public ProviderRequest Request { get; set; }

    public CreateProviderCommand(ProviderRequest request)
    {
        Request = request;
    }
}