using FluentValidation;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class ProviderRequestValidator : AbstractValidator<ProviderRequest>
{
    public ProviderRequestValidator()
    {
        Include(new UserRequestValidator());
        
        RuleFor(c => c.Rif)
            .NotEmpty().WithMessage("El rif es requerido")
            .Matches(@"^[JVDG]\d{9}$")
            .WithMessage(
                "El rif debe seguir el formato V123456789. Si el RIF es menor a 9 dígitos complete con 0 a la izquierda");
        
        RuleFor(c => c.AccountNumber)
            .NotEmpty().WithMessage("El número de cuenta es requerido")
            .Matches(@"^\d{20}$").WithMessage("El número de cuenta debe tener 20 dígitos");
    }
}