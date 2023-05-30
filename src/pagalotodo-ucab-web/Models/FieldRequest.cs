using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class FieldRequest
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public int? Length { get; set; }
    
    [Required]
    public string? Format { get; set; }
    
    public Guid? Service { get; set; }
    
    [Required]
    public string? AttrReference { get; set; }
}