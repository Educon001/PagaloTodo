using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Services;

public class GetServicesByProviderIdQuery : IRequest<List<ServiceResponse>>
{
    public Guid Id { get; set; }

    public GetServicesByProviderIdQuery(Guid id)
    {
        Id = id;
    }
}