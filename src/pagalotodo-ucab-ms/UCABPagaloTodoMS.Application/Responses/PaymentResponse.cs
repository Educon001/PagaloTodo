using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Responses;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public double? Amount { get; set; }
    public string? CardholderName { get; set; }
    public string? CardNumber { get; set; }
    public string? TransactionId { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerResponse? Consumer { get; set; }
    public ServiceResponse? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}