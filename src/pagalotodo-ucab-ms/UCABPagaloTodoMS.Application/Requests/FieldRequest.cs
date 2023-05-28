namespace UCABPagaloTodoMS.Application.Requests;

public class FieldRequest
{
    public string? Name { get; set; }
    public uint? Length { get; set; }
    public string? Format { get; set; }
    public Guid? Service { get; set; }
    public string? AttrReference { get; set; }
}