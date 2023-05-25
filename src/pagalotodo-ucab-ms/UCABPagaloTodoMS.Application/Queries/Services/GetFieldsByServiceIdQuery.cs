using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Services;

public class GetFieldsByServiceIdQuery : IRequest<List<FieldResponse>>
{
    public Guid? Id { get; set; }

    public GetFieldsByServiceIdQuery(Guid id)
    {
        Id = id;
    }
}