using MediatR;
using UCABPagaloTodoMS.Application.Responses;

namespace UCABPagaloTodoMS.Application.Queries.Payments;

public class GetPaymentsQuery : IRequest<List<PaymentResponse>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetPaymentsQuery(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}