namespace UCABPagaloTodoMS.Core.Entities;

public class FieldEntity : BaseEntity
{
    public string? Name { get; set; }
    public uint? Length { get; set; }
    public string? Format { get; set; }
}