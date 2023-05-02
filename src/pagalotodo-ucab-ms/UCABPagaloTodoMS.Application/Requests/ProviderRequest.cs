namespace UCABPagaloTodoMS.Application.Requests;

public class ProviderRequest : UserRequest
{
    public string? Rif { get; set; }
    public string? AccountNumber { get; set; }
}