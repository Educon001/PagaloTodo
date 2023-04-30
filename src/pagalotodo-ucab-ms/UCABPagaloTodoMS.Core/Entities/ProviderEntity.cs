namespace UCABPagaloTodoMS.Core.Entities;

public class ProviderEntity : UserEntity
{
    public string? Rif { get; set; }
    public string? AccountNumber { get; set; }
    public List<ServiceEntity>? Services { get; set; }
}