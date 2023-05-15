namespace UCABPagaloTodoMS.Application.Responses;

public class ConsumerResponse : UserResponse
{
    public string? ConsumerId { get; set; }
    public List<PaymentResponse>? Payments { get; set; }
}