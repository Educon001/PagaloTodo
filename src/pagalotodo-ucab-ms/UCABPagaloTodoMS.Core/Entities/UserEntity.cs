using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UCABPagaloTodoMS.Core.Entities;

[Index(nameof(Username),IsUnique = true)]
[Index(nameof(Email),IsUnique = true)]
public class UserEntity : BaseEntity
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool? Status { get; set; }
}