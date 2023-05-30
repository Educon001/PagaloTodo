using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Services;

public class GetFieldByIdQuery : IRequest<FieldResponse>
{
    public Guid? Id { get; set; }
    public GetFieldByIdQuery(Guid id)
    {
        Id = id;
    }
}