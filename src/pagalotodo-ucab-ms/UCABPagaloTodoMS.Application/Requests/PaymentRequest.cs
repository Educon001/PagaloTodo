using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Requests;

public class PaymentRequest
{
    public double? Amount { get; set; }
    public string? CardNumber { get; set; }
    public int? ExpirationMonth { get; set; }
    public int? ExpirationYear { get; set; }
    public string? CardholderName { get; set; }
    public string? CardSecurityCode { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public Guid? Consumer { get; set; }
    public Guid? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}