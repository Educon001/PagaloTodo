using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands.Payments;

public class CreatePaymentDetailCommand : IRequest<Guid>
{
    public PaymentDetailRequest Request { get; set; }

    public CreatePaymentDetailCommand(PaymentDetailRequest request)
    {
        Request = request;
    }
}