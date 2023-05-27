using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class DeleteConsumerCommandHandler : IRequestHandler<DeleteConsumerCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<DeleteConsumerCommandHandler> _logger;

    public DeleteConsumerCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<DeleteConsumerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(DeleteConsumerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    private async Task<Guid> HandleAsync(DeleteConsumerCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("DeleteConsumerCommandHandler.HandleAsync {Request}", request);
            var consumerId = request.Request;
            var entity = _dbContext.Consumers.Find(consumerId);
            if (entity != null)
            {
                _dbContext.Consumers.Remove(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Object with key {consumerId} not found");
            }

            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            _logger.LogInformation("DeleteConsumerCommandHandler.HandleAsync {Response}", consumerId);
            return consumerId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error DeleteConsumerCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}