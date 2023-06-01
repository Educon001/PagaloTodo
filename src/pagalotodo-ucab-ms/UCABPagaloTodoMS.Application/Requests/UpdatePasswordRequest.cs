namespace UCABPagaloTodoMS.Application.Requests;

public class UpdatePasswordRequest
{
    public string? PasswordHash { get; set; }
    public string? UserType { get; set; }
}