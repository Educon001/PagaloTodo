using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands;

public class CreateConsumerCommand : IRequest<Guid>
{
    public ConsumerRequest Request { get; set; }

    public CreateConsumerCommand(ConsumerRequest request)
    {
        Request = request;
    }
}