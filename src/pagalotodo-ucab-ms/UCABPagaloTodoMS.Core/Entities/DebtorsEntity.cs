using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoMS.Core.Entities;

public class DebtorsEntity : BaseEntity
{
    public string? Identifier { get; set; }
    public double? Amount { get; set; }
    public bool? Status { get; set; }
    [Required]
    public ServiceEntity? Service { get; set; }
}