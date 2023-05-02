namespace UCABPagaloTodoMS.Application.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public bool? Status { get; set; }
}