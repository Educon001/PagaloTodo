using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreateProviderCommandHandler> _logger;

    public CreateProviderCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<CreateProviderCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreateProviderCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            var validator = new ProviderRequestValidator();
            validator.ValidateAndThrow(request.Request);
            return await HandleAsync(request);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<Guid> HandleAsync(CreateProviderCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("CreateProviderCommandHandler.HandleAsync {Request}", request);
            var entity = ProviderMapper.MapRequestToEntity(request.Request);
            _dbContext.Providers.Add(entity);
            var id = entity.Id;
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            _logger.LogInformation("CreateProviderCommandHandler.HandleAsync {Response}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreateProviderCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}