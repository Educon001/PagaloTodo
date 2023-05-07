using MediatR;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class ConsultarPagosQueryHandler : IRequestHandler<ConsultarPagosQuery, List<PaymentResponse>>
{
    public Task<List<PaymentResponse>> Handle(ConsultarPagosQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}