using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Requests;

public class ServiceRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ServiceStatusEnum ServiceStatus { get; set; }
    public ServiceTypeEnum ServiceType { get; set; }
    public Guid Provider { get; set; }
}