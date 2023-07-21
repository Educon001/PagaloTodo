using FluentValidation;
using FluentValidation.Validators;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;

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
            .NotEmpty().WithMessage("Debe Incluir el Atributo de Referencia")
            .Matches(@"^(?i)(payment\.|consumer\.|paymentdetail\.)\w+$")
            .Must(ValidAttribute).WithMessage("Atributo invalido");
    }

    /// <summary>
    /// Validar si el campo es de las clases payment, consumer o paymentdetail y validar que en dicha tabla este el atributo
    /// </summary>
    /// <param name="attr"></param>
    /// <returns>bool</returns>
    private bool ValidAttribute(string attr)
    {
        var table = attr.Split(".")[0];
        var attribute = attr.Split(".")[1];

        switch (table.ToLower())
        {
            case "payment":
                var pProperties = typeof(PaymentResponse).GetProperties().Select(p => p.Name.ToLower()).ToList();
                return pProperties.Contains(attribute);
            case "consumer":
                var cProperties = typeof(ConsumerResponse).GetProperties().Select(p => p.Name.ToLower()).ToList();
                return cProperties.Contains(attribute);
            case "paymentdetail":
                return true;
            default:
                return false;
        }
    }
}