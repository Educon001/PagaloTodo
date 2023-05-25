using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdatePasswordCommandHandler> _logger;

    public UpdatePasswordCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<UpdatePasswordCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("UpdatePasswordCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            var validator = new UpdatePasswordRequestValidator();
            validator.ValidateAndThrow(request.Request);
            return await HandleAsync(request);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<UpdatePasswordResponse> HandleAsync(UpdatePasswordCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdatePasswordCommandHandler.HandleAsync {Request}", request);
            var consumerId = request.Id;
            var entity = _dbContext.Consumers.Find(consumerId);
            if (entity != null)
            {
                if (request.Request.PasswordHash != null)
                {
                    entity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                }
                _dbContext.Consumers.Update(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Object with key {consumerId} not found");
            }
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            //var response = ConsumerMapper.MapEntityToResponse(entity);
            var response = new UpdatePasswordResponse(entity.Id, "Password updated successfully.");
            _logger.LogInformation("UpdatePasswordCommandHandler.HandleAsync {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdatePasswordCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}