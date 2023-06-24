namespace UCABPagaloTodoMS.Application.Requests;

public class DebtorsRequest
{
    public string? Identifier { get; set; }
    public double? Amount { get; set; }
    public bool? Status { get; set; }
}