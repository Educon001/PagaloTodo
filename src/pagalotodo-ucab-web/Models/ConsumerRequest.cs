using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class ConsumerRequest : UserRequest
{
    [Required(ErrorMessage = "Cedula requerida")]
    [RegularExpression("^[VEGP].{1,8}$", ErrorMessage = "Formato de Cedula invalido")]
    public string? ConsumerId { get; set; }
    
    [Required(ErrorMessage = "Apellidos requerido")]
    public string? LastName { get; set; }
}