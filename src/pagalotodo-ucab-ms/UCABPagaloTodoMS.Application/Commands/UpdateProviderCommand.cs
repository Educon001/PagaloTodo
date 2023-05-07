using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands;

public class UpdateProviderCommand : IRequest<ProviderResponse>
{
    public ProviderRequest Request { get; set; }
    public Guid Id { get; set; }

    public UpdateProviderCommand(ProviderRequest request, Guid id)
    {
        Request = request;
        Id = id;
    }
}