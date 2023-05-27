using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Services;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceResponse>
{
    
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdateServiceCommandHandler> _logger;

    public UpdateServiceCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<UpdateServiceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<ServiceResponse> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
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
    
    private async Task<ServiceResponse> HandleAsync(UpdateServiceCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdateServiceCommandHandler.HandleAsync {Request}", request);
            var serviceId = request.Id;
            var entity = _dbContext.Services.Find(serviceId);
            // var entity = ServiceMapper.MapRequestToEntity(request.Request);
            // si la entidad no es nula
            if (entity is not null)
            {
                entity.Name = request.Request.Name;
                entity.Description = request.Request.Description;
                entity.ServiceStatus = request.Request.ServiceStatus ?? 0;
                entity.ServiceType = request.Request.ServiceType ?? 0;
                _dbContext.Services.Update(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                _logger.LogInformation("UpdateServiceCommandHandler.HandleAsync {Response}", entity.Id);
            }
            else
            {
                throw new NotImplementedException();
            }
            return ServiceMapper.MapEntityToResponse(entity,false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdateServiceCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}