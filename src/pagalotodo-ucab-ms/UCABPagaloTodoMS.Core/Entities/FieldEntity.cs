using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UCABPagaloTodoMS.Core.Entities;

public class FieldEntity : BaseEntity
{
    public string? Name { get; set; }
    public uint? Length { get; set; }
    public string? Format { get; set; }
    [Required]
    public ServiceEntity? Service { get; set; }

    public string? AttrReference { get; set; }
}