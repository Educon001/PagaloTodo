using System.ComponentModel.DataAnnotations;
namespace UCABPagaloTodoWeb.Models;

public class UpdatePasswordRequest
{

    [Required(ErrorMessage = "Contraseña requerida")]
    [RegularExpression(".([a-z]|[A-Z]|[!@#$%^&(),.?\":{}|<>-]).*", ErrorMessage = "La contraseña debe tener al menos una minúscula, una mayúscula y un caracte especial")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener minimo 8 caracteres")]
    public string? PasswordHash { get; set; }
    public string? UserType { get; set; }
}