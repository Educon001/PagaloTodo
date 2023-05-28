namespace UCABPagaloTodoMS.Core.Entities;

public class ConsumerEntity : UserEntity
{
    public string? LastName { get; set; }
    public string? ConsumerId { get; set; }
    public List<PaymentEntity>? Payments { get; set; }
}