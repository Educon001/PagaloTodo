using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoWeb.Models;

public class PaymentModel
{
    public Guid Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public float? Amount { get; set; }
    public string? CardholderName { get; set; }
    public string? CardNumber { get; set; }
    public string? TransactionId { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerModel? Consumer { get; set; }
    public ServiceModel? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
}