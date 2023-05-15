using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class GetServicesByIdQuery : IRequest<ServiceResponse>
{
    public Guid? Id { get; set; }
    public GetServicesByIdQuery(Guid id)
    {
        Id = id;
    }
}