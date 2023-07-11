using System.ComponentModel.DataAnnotations;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Core.Entities;

public class PaymentEntity : BaseEntity
{
    public double? Amount { get; set; }
    public string? CardNumber { get; set; }
    public int? ExpirationMonth { get; set; }
    public int? ExpirationYear { get; set; }
    public string? CardholderName { get; set; }
    public string? CardSecurityCode { get; set; }
    public string? TransactionId { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public ConsumerEntity? Consumer { get; set; }
    public ServiceEntity? Service { get; set; }
    //Para los pagos por confirmacion
    public string? Identifier { get; set; }
    
    public List<PaymentDetailEntity>? PaymentDetails { get; set; }
}