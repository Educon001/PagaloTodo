namespace UCABPagaloTodoMS.Application.Requests;

public class LoginRequest
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? UserType { get; set; }
}