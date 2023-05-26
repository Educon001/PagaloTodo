using MediatR;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Commands.Payments;

public class UpdatePaymentStatusCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public PaymentStatusEnum? NewPaymentStatus { get; set; }

    public UpdatePaymentStatusCommand(Guid id, PaymentStatusEnum? newPaymentStatus)
    {
        Id = id;
        NewPaymentStatus = newPaymentStatus;
    }
}