using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Responses;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public float? Amount { get; set; }
    public string? OriginAccount { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerResponse? Consumer { get; set; }
    public ServiceResponse? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}