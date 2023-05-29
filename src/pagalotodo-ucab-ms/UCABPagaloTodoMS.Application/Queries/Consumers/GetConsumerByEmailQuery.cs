using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Consumers;

public class GetConsumerByEmailQuery : IRequest<Guid>
{
    public string Email { get; set; }

    public GetConsumerByEmailQuery(string email)
    {
        Email = email;
    }
}