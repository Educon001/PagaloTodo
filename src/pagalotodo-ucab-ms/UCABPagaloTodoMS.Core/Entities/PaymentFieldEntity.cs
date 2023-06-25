using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoMS.Core.Entities;

public class PaymentFieldEntity : BaseEntity
{
    public string? Name { get; set; }
    
    public string? Format { get; set; }
    
    [Required]
    public ServiceEntity? Service { get; set; }
}