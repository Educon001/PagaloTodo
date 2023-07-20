using FluentValidation;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class PaymentDetailValidator : AbstractValidator<PaymentDetailRequest>
{
    private readonly string _type;
    
    public PaymentDetailValidator(string type)
    {
        _type = type;
        RuleFor(p => p.Value)
            .NotEmpty().WithMessage("El valor no puede estar vacio")
            .Must(BeValidType!).WithMessage("Tipo de dato inválido");
    }

    private bool BeValidType(string value)
    {
        switch (_type.ToLower())
        {
            case "int":
                return int.TryParse(value, out _);
            case "date":
                return DateTime.TryParse(value, out _);
            case "double":
                return double.TryParse(value, out _);
            case "string":
                return true;
            default:
                return true;
        }
    }
}