namespace UCABPagaloTodoWeb.Models;

public class ProviderModel : UserModel
{
    public string? Rif { get; set; }

    public string? AccountNumber { get; set; }
    
    public List<ServiceModel>? Services { get; set; }
}