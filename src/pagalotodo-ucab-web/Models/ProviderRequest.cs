using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class ProviderRequest : UserRequest
{
    [Required(ErrorMessage = "Rif Required")]
    [RegularExpression(@"^[JVDG]\d{9}$", ErrorMessage = "Rif format invalid")]
    public string? Rif { get; set; }
    
    [Required(ErrorMessage = "Numero de Cuenta Required")]
    [RegularExpression(@"^\d{20}$", ErrorMessage = "Numero de cuenta format invalid")]
    public string? AccountNumber { get; set; }
}