using MediatR;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries;

public class ConsultarPagosQuery : IRequest<List<PaymentResponse>>
{
    
}