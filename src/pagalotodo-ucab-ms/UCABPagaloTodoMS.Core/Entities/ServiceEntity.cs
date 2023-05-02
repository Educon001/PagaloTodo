using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Core.Entities;

public class ServiceEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ServiceStatusEnum ServiceStatus { get; set; }
    public ServiceTypeEnum ServiceType { get; set; }
    public ProviderEntity? Provider { get; set; }
    public List<PaymentEntity>? Payments { get; set; }
    public List<DebtorsEntity>? ConfirmationList { get; set; }
    public List<FieldEntity>? ConciliationFormat { get; set; }
}