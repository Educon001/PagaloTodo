using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Services;

public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreateFieldCommandHandler> _logger;

    public CreateFieldCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<CreateFieldCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        
        if (request.Request == null) 
        {
            _logger.LogWarning("CreateFieldCommandHandler.Handle: Request nulo.");
            throw new ArgumentNullException(nameof(request));
        }
        return await HandleAsync(request);
    }
    
    private async Task<Guid> HandleAsync(CreateFieldCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            if (_dbContext.Services.Find(request.Request.Service) is not null)
            {
                _logger.LogInformation("CreateFieldCommandHandler.HandleAsync {Request}", request);
                var entity = FieldMapper.MapRequestToEntity(request.Request, _dbContext);
                
                //fields entity add
                _dbContext.Fields.Add(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                
                //service entity modify
                // ServiceEntity serviceEntity = entity.Service!;
                // serviceEntity.ConciliationFormat!.Add(FieldMapper.MapRequestToEntity(request.Request, _dbContext)); 
                // _dbContext.Services.Update(serviceEntity);
                // await _dbContext.SaveEfContextChanges("APP");
                // transaccion.Commit();
                
                _logger.LogInformation("CreateFieldCommandHandler.HandleAsync {Response}", entity.Id);
                return entity.Id;
            }
            throw new Exception($"Servicio con id: {request.Request.Service} no se encuentra en la base de datos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreateFieldCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}