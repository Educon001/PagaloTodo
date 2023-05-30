using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El campo Username es requerido.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El campo PasswordHash es requerido.")]
        public string PasswordHash { get; set; }

        public string? UserType { get; set; }
    }
}