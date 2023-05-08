using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Application.Validators;

public class AddServiceValidator : AbstractValidator<ServiceRequest>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    
    public AddServiceValidator(/*IUCABPagaloTodoDbContext dbContext*/)
    {
        // _dbContext = dbContext;
        
        // Rules for validation
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nombre es requerido");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descripcion es requerida");
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Debe especificar un proveedor");
        RuleFor(x => x.ServiceStatus)
            .NotNull().WithMessage("Debe especificar un estatus de servicio");
        RuleFor(x => x.ServiceStatus)
            .IsInEnum().WithMessage("Debe especificar un estatus de servicio valido");
        RuleFor(x => x.ServiceType)
            .NotNull().WithMessage("Debe especificar un tipo de servicio");
        RuleFor(x => x.ServiceType)
            .IsInEnum().WithMessage("Debe especificar un tipo de servicio valido");
        // RuleFor(x => x.Request.Provider)
        //     .Must(validateProviderExists).WithMessage("Proveedor no existe en la base de datos.");
    }

    private bool validateProviderExists(Guid providerGuid)
    {
        return _dbContext.Providers.Find(providerGuid) is not null;
    }
}