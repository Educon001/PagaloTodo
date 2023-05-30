namespace UCABPagaloTodoWeb.Models;

public class FieldModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public int? Length { get; set; }

    public string? Format { get; set; }
    
    public string? AttrReference { get; set; }
}