using FluentValidation;
using UCABPagaloTodoMS.Application.Requests;

namespace UCABPagaloTodoMS.Application.Validators;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(p => p.Amount)
            .NotEmpty().WithMessage("El monto a pagar es requerido y debe ser mayor a 0");

        RuleFor(p => p.CardNumber)
            .NotEmpty().WithMessage("El numero de tarjeta es requerido")
            .Matches(@"^\d{13,18}$").WithMessage("Introduzca un número de tarjeta valido");

        RuleFor(p => p.CardholderName)
            .NotEmpty().WithMessage("El nombre del tarjetahabiente es requerido")
            .Matches(@"^[A-Za-z ]+$").WithMessage("El nombre solo puede contener letras");

        RuleFor(p => p.ExpirationYear)
            .NotNull().WithMessage("El año de expiración es requerido")
            .Must(n=>n>=0 && n<=99 || n>=2000 && n<=2099).WithMessage("Introduzca un año válido");

        RuleFor(p => p.ExpirationMonth)
            .NotNull().WithMessage("El mes de expiración es requerido")
            .InclusiveBetween(1, 12).WithMessage("Introduzca un mes válido");

        RuleFor(p => p.CardSecurityCode)
            .NotEmpty().WithMessage("El cvv es requerido")
            .Matches(@"^\d{3,4}$").WithMessage("El cvv debe tener 3 o 4 digitos");
        
        RuleFor(p => p.PaymentStatus)
            .NotNull().WithMessage("El estado del pago es requerido")
            .IsInEnum().WithMessage("Debe especificar un estado de pago valido");

        RuleFor(p => p.Consumer)
            .NotEmpty().WithMessage("El consumidor es requerido");

        RuleFor(p => p.Service)
            .NotEmpty().WithMessage("El servicio es requerido");

        RuleFor(p => p.Identifier)
            .NotEmpty().When(p => p.Identifier is not null)
            .WithMessage("El identificador no puede estar vacio");
    }
}