using System.Reflection;

namespace UCABPagaloTodoMS.Core.Entities;

public class FieldEntity : BaseEntity
{
    public string? Name { get; set; }
    public uint? Length { get; set; }
    public string? Format { get; set; }
    public ServiceEntity? Service { get; set; }

    public string? Type { get; set; }
    
    public string? AttrReference { get; set; }
}