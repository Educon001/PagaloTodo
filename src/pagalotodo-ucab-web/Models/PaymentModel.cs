namespace UCABPagaloTodoWeb.Models;

public class PaymentModel
{
    public float? Amount { get; set; }

    public string? OriginAccount { get; set; }

    public string? PaymentStatus { get; set; }

    public ConsumerModel Consumer { get; set; }

    public ServiceModel Service { get; set; }

    public string? Identifier { get; set; }
}