using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoMS.Core.Entities;

public class UserEntity : BaseEntity
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public bool? Status { get; set; }
}