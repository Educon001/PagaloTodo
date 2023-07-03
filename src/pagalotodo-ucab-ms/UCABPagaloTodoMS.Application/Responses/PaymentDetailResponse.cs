namespace UCABPagaloTodoMS.Application.Responses;

public class PaymentDetailResponse
{
    public string? Name { get; set; }

    public string? Value { get; set; }
    
    public PaymentResponse? Payment { get; set; }
}