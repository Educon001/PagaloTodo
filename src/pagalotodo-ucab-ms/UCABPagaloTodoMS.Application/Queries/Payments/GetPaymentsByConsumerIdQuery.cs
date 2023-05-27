using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Payments;

public class GetPaymentsByConsumerIdQuery : IRequest<List<PaymentResponse>>
{
    public Guid Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetPaymentsByConsumerIdQuery(Guid id, DateTime? startDate, DateTime? endDate)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
    }
}