using FluentValidation;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(c => c.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido");
        
        RuleFor(c => c.PasswordHash)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[!@#$%^&*(),.?\":{}|<>-]").WithMessage("La contraseña debe contener al menos un carácter especial.");
        
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("El correo es requerido")
            .EmailAddress().WithMessage("El correo debe tener un formato válido");
        
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("El nombre es requerido");
        
        RuleFor(c => c.LastName)
            .NotEmpty().When(c => c.LastName != null)
            .WithMessage("El apellido no puede estar vacío");
        
        RuleFor(c => c.Status)
            .NotNull().WithMessage("El estado de la cuenta es requerido");
    }
}