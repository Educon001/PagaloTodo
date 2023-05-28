using FluentValidation;
using FluentValidation.Validators;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class FieldValidator : AbstractValidator<FieldRequest>
{
    public FieldValidator()
    {
        RuleFor(c => c.Format)
            .NotEmpty().WithMessage("Formato vacio");
        RuleFor(c => c.Length)
            .NotEmpty().WithMessage("Longitud del campo no valida");
        //TODO: Añadir validacion de referencia para que solo pueda ser a pagos o a consumidor.
    }
}