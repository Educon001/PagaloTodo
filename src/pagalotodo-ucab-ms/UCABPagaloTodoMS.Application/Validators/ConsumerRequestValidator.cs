using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Validators;

public class ConsumerRequestValidator : AbstractValidator<ConsumerRequest>
{

    public ConsumerRequestValidator()
    {

        Include(new UserRequestValidator());

        RuleFor(c => c.ConsumerId)
            .NotEmpty().WithMessage("La cédula es requerida")
            .Matches(@"^[VEGP].{1,8}$").WithMessage("La cédula debe tener formato V12345678");

    }
}