using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Services;

public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FieldResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdateFieldCommandHandler> _logger;

    public UpdateFieldCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<UpdateFieldCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<FieldResponse> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
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

    /// <summary>
    /// Handles the update of a field in the database.
    /// </summary>
    /// <param name="request">The request containing the field information to update.</param>
    /// <returns>The updated field information.</returns>
    private async Task<FieldResponse> HandleAsync(UpdateFieldCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdateFieldCommandHandler.HandleAsync {Request}", request);
            var fieldId = request.Id;
            var entity = _dbContext.Fields.Find(fieldId);
            // si la entidad no es nula
            if (entity is not null)
            {
                entity.Name = request.Request.Name;
                entity.Format = request.Request.Format;
                entity.Length = request.Request.Length;
                entity.AttrReference = request.Request.AttrReference;
                _dbContext.Fields.Update(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                _logger.LogInformation("UpdateFieldCommandHandler.HandleAsync {Response}", entity.Id);
            }
            else
            {
                throw new NotImplementedException();
            }

            return FieldMapper.MapEntityToResponse(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdateFieldCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}