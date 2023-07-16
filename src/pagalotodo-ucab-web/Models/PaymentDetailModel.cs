namespace UCABPagaloTodoWeb.Models;

public class PaymentDetailModel
{
    public string? Name { get; set; }

    public string? Value { get; set; }
    
    public PaymentModel? Payment { get; set; }
}