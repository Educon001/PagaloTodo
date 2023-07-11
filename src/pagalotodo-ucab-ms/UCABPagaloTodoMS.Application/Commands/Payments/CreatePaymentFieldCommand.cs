using MediatR;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Commands.Payments;

public class CreatePaymentFieldCommand : IRequest<Guid>
{
    public PaymentFieldRequest Request { get; set; }

    public CreatePaymentFieldCommand(PaymentFieldRequest request)
    {
        Request = request;
    }
}