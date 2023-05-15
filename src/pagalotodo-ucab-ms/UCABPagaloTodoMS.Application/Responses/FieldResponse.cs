namespace UCABPagaloTodoMS.Application.Responses;

public class FieldResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public uint? Length { get; set; }
    public string? Format { get; set; }
    
    public string? Type { get; set; }
    
    public string? AttrReference { get; set; }
}