using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Validators;

public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
{

    public UpdatePasswordRequestValidator()
    {

        RuleFor(c => c.PasswordHash)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[!@#$%^&*(),.?\":{}|<>-]").WithMessage("La contraseña debe contener al menos un carácter especial.");
    }
}