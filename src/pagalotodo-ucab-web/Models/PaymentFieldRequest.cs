namespace UCABPagaloTodoWeb.Models;

public class PaymentFieldRequest
{
    public string? Name { get; set; }
    
    public string? Format { get; set; }
    
    public Guid? Service { get; set; }
}