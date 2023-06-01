using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
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
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    private async Task<UpdatePasswordResponse> HandleAsync(UpdatePasswordCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();

        try
        {
            _logger.LogInformation("UpdatePasswordCommandHandler.HandleAsync {Request}", request);

            switch (request.Request.UserType)
            {
                case "consumer":
                    var consumerId = request.Id;
                    var consumerEntity = _dbContext.Consumers.Find(consumerId);
                    if (consumerEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            consumerEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Consumers.Update(consumerEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {consumerId} not found");
                    }
                    break;
                case "provider":
                    var providerId = request.Id;
                    var providerEntity = _dbContext.Providers.Find(providerId);
                    if (providerEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            providerEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Providers.Update(providerEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {providerId} not found");
                    }
                    break;
                case "admin":
                    var adminId = request.Id;
                    var adminEntity = _dbContext.Admins.Find(adminId);
                    if (adminEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            adminEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Admins.Update(adminEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {adminId} not found");
                    }
                    break;
            }

            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();

            var response = new UpdatePasswordResponse(request.Id, "Password updated successfully.");
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