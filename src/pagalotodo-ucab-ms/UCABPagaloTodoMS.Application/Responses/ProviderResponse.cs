namespace UCABPagaloTodoMS.Application.Responses;

public class ProviderResponse : UserResponse
{
    public string? Rif { get; set; }
    public string? AccountNumber { get; set; }
    public List<ServiceResponse>? Services { get; set; }
}