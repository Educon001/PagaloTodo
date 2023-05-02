namespace UCABPagaloTodoMS.Core.Entities;

public class DebtorsEntity : BaseEntity
{
    public string? Identifier { get; set; }
    public float? Amount { get; set; }
    public bool? Status { get; set; }
    public ServiceEntity? Service { get; set; }
}