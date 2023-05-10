using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Commands;

public class UpdateConsumerCommand : IRequest<ConsumerResponse>
{
    public ConsumerRequest Request { get; set; }
    public Guid Id { get; set; }

    public UpdateConsumerCommand(ConsumerRequest request, Guid id)
    {
        Request = request;
        Id = id;
    }
}