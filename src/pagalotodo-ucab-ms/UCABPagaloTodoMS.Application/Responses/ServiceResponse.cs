using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Responses;

public class ServiceResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string ServiceStatus { get; set; }
    public string ServiceType { get; set; }
    public ProviderResponse? Provider { get; set; }
    public List<PaymentResponse>? Payments { get; set; }
    public List<DebtorsResponse>? ConfirmationList { get; set; }
    public List<FieldResponse>? ConciliationFormat { get; set; }
    
    public List<PaymentFieldResponse> PaymentFormat { get; set; }
}