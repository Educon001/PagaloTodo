namespace UCABPagaloTodoMS.Application.Responses;

public class DebtorsResponse
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public double? Amount { get; set; }
    public bool? Status { get; set; }
}