using FluentValidation;
using FluentValidation.Validators;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class FieldValidator : AbstractValidator<FieldRequest>
{
    public FieldValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Nombre vacio");
        RuleFor(c => c.Service)
            .NotEmpty().WithMessage("Debe Incluir un Servicio");
        RuleFor(c => c.AttrReference)
            .NotEmpty().WithMessage("Debe Incluir el Atributo de Referencia");
        RuleFor(c => c.Format)
            .NotEmpty().WithMessage("Formato vacio");
        RuleFor(c => c.Length)
            .NotEmpty().WithMessage("Longitud del campo no valida");
        //TODO: Añadir validacion de referencia para que solo pueda ser a pagos o a consumidor.
    }
}