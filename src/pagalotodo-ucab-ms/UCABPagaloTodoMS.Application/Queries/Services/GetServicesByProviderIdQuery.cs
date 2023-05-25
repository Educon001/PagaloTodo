using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class GetServicesByProviderIdQuery : IRequest<List<ServiceResponse>>
{
    public Guid Id { get; set; }

    public GetServicesByProviderIdQuery(Guid id)
    {
        Id = id;
    }
}