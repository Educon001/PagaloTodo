using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class ConsumersQuery : IRequest<List<ConsumerResponse>>
{

}