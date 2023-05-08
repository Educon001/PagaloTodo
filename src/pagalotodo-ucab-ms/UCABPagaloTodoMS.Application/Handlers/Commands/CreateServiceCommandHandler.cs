using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class CreateServiceCommandHandler  : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreateServiceCommandHandler> _logger;

    public CreateServiceCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<CreateServiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Request == null) 
        {
                _logger.LogWarning("CreateServiceCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
        }
        return await HandleAsync(request);
    }
    
    private async Task<Guid> HandleAsync(CreateServiceCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            if (_dbContext.Providers.Find(request.Request.Provider) is not null)
            {
                _logger.LogInformation("CreateServiceCommandHandler.HandleAsync {Request}", request);
                var entity = ServiceMapper.MapRequestToEntity(request.Request, _dbContext);
                _dbContext.Services.Add(entity);
                var id = entity.Id;
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                _logger.LogInformation("CreateServiceCommandHandler.HandleAsync {Response}", id);
                return id;
            }
            throw new Exception($"Proveedor con id: {request.Request.Provider} no se encuentra en la base de datos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreateServiceCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}