using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class FieldRequest
{
    [Required]
    public string? Name { get; set; }
    
    public int? Length { get; set; }
    
    public string? Format { get; set; }
    
    public Guid? Service { get; set; }
    
    [Required]
    public string? AttrReference { get; set; }
}