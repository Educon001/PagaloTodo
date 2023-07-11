namespace UCABPagaloTodoMS.Core.Entities;

public class PaymentDetailEntity : BaseEntity
{
    public string? Name { get; set; }

    public string? Value { get; set; }
    
    public PaymentEntity? Payment { get; set; }
}