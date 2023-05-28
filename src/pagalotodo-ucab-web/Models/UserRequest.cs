using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace UCABPagaloTodoWeb.Models;

public class UserRequest
{
    
    [Remote(action: "checkUsername", controller: "Consumer", HttpMethod ="GET", ErrorMessage = "Username ya existe")]
    [Required(ErrorMessage = "Username requerido")]
    public string? Username { get; set; }
    
    // [Required(ErrorMessage = "Contraseña requerida")]
    public string? PasswordHash { get; set; }
    
    [Required(ErrorMessage = "Email requerido")]
    [EmailAddress(ErrorMessage = "No corresponde a una direccion de correo")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Nombre(s) requerido")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Apellidos requerido")]
    public string? LastName { get; set; }
    public bool? Status { get; set; }
}