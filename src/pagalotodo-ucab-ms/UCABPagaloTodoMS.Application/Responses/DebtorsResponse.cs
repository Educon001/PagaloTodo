namespace UCABPagaloTodoMS.Application.Responses;

public class DebtorsResponse
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public float? Amount { get; set; }
    public bool? Status { get; set; }
}