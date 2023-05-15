using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Services;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<DeleteServiceCommandHandler> _logger;

    public DeleteServiceCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<DeleteServiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request);
    }
    
    private async Task<Guid> HandleAsync(DeleteServiceCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("DeleteServiceCommandHandler.HandleAsync {Request}", request);
            var entityId = request.Id;
            var entity = _dbContext.Services.Find(entityId);
            if (entity is not  null)
            {
                _dbContext.Services.Remove(entity);
            }
            else
            {
                throw new Exception($"Servicio {entityId} no se encontro en la base de datos");
            }
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            _logger.LogInformation("DeleteServiceCommandHandler.HandleAsync {Response}", entityId);
            return entityId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error DeleteServiceCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}