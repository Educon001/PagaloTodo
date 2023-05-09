using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Validators;

public class ConsumerRequestValidator : AbstractValidator<ConsumerRequest>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;

    public ConsumerRequestValidator(IUCABPagaloTodoDbContext dbContext)
    {
        _dbContext = dbContext;

        Include(new UserRequestValidator(_dbContext));

        RuleFor(c => c.ConsumerId)
            .NotEmpty().WithMessage("La cédula es requerida")
            .Matches(@"^[VEGP].{7,8}$").WithMessage("La cédula debe tener formato V1234567 o V12345678.")
            .MustAsync(async (consumerId, cancellationToken) =>
            {
                return !await _dbContext.Consumers.AnyAsync(c => c.ConsumerId == consumerId, cancellationToken);
            }).WithMessage(request => $"La cédula '{request.ConsumerId}' ya está asociada a una cuenta registrada");

        // otras reglas de validación
    }
}