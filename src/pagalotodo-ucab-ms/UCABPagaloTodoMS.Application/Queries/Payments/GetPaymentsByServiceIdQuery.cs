using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Payments;

public class GetPaymentsByServiceIdQuery : IRequest<List<PaymentResponse>>
{
    public Guid Id { get; set; }

    public GetPaymentsByServiceIdQuery(Guid id)
    {
        Id = id;
    }
}