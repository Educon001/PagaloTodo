using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class ServiceModel
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }

    public string? ServiceStatus { get; set; }

    public string? ServiceType { get; set; }

    public ProviderModel? Provider { get; set; }
    
    public List<PaymentModel>? Payments { get; set; }

    public List<DebtorsModel>? ConfirmationList { get; set; }
    
    public List<FieldModel>? ConciliationFormat { get; set; }
}