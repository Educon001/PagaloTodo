namespace UCABPagaloTodoMS.Application.Responses;

public class ConsumerResponse : UserResponse
{
    public uint ConsumerId { get; set; }
    public List<PaymentResponse>? Payments { get; set; }
}