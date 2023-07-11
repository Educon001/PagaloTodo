using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoWeb.Models;

public class ConfirmationList
{
    [Required(ErrorMessage = "Please select a file.")]
    public IFormFile File { get; set; }
}