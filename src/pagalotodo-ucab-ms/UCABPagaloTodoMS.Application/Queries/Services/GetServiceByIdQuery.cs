using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class GetServiceByIdQuery : IRequest<ServiceResponse>
{
    public Guid? Id { get; set; }
    public GetServiceByIdQuery(Guid id)
    {
        Id = id;
    }
}