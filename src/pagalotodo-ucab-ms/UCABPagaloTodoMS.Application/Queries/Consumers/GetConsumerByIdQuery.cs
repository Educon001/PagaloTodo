using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Providers;

public class GetConsumerByIdQuery : IRequest<ConsumerResponse>
{
    public Guid Id { get; set; }

    public GetConsumerByIdQuery(Guid id)
    {
        Id = id;
    }
}