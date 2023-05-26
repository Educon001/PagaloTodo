using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands.Payments;

public class CreatePaymentCommand : IRequest<Guid>
{
    public PaymentRequest Request { get; set; }

    public CreatePaymentCommand(PaymentRequest request)
    {
        Request = request;
    }
}