namespace UCABPagaloTodoMS.Core.Entities;

public class ConsumerEntity : UserEntity
{
    public uint ConsumerId { get; set; }
    public List<PaymentEntity>? Payments { get; set; }
}