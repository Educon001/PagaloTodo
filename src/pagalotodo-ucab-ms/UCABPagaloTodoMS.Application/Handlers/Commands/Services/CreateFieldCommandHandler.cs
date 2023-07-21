using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
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
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreateFieldCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the update of a field in the database.
    /// </summary>
    /// <param name="request">The request containing the field information to update.</param>
    /// <returns>The ID of the updated field.</returns>
    private async Task<Guid> HandleAsync(CreateFieldCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            if (_dbContext.Services.Find(request.Request.Service) is not null)
            {
                _logger.LogInformation("CreateFieldCommandHandler.HandleAsync {Request}", request);
                ServiceEntity? serviceE = _dbContext.Services.Find(request.Request.Service);
                var entity = FieldMapper.MapRequestToEntity(request.Request, serviceE!);

                //fields entity add
                _dbContext.Fields.Add(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                

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