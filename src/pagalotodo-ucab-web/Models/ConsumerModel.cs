namespace UCABPagaloTodoWeb.Models;

public class ConsumerModel : UserModel
{
    public string? ConsumerId { get; set; }

    public List<PaymentModel> Payments { get; set; }
    
    public string? LastName { get; set; }
}