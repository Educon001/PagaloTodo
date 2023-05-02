namespace UCABPagaloTodoMS.Application.Requests;

public class DebtorsRequest
{
    public string? Identifier { get; set; }
    public float? Amount { get; set; }
    public bool? Status { get; set; }
}