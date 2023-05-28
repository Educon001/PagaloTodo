using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoMS.Core.Entities;

public class UserEntity : BaseEntity
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool? Status { get; set; }
}