using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Payments;

public class GetPaymentFieldsByServiceIdQuery : IRequest<List<PaymentFieldResponse>>
{
    public Guid? Id { get; set; }
    
    public GetPaymentFieldsByServiceIdQuery(Guid id)
    {
        Id = id;
    }
}