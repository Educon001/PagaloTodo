using System.ComponentModel.DataAnnotations;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoWeb.Models;

public class ServiceRequest
{
    [Required(ErrorMessage = "Nombre requerido")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Descripcion requerida")]
    public string? Description { get; set; }
    public ServiceStatusEnum? ServiceStatus { get; set; }
    public ServiceTypeEnum? ServiceType { get; set; }
    public Guid Provider { get; set; }
}