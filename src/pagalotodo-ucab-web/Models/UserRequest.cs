namespace UCABPagaloTodoWeb.Models;

public class UserRequest
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public bool? Status { get; set; }
}