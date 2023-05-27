using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Providers;

public class GetProviderByIdQuery : IRequest<ProviderResponse>
{
    public Guid Id { get; set; }

    public GetProviderByIdQuery(Guid id)
    {
        Id = id;
    }
}