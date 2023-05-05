using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Requests;

public class PaymentRequest
{
    public float? Amount { get; set; }
    public string? OriginAccount { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerRequest? Consumer { get; set; }
    public ServiceRequest? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}