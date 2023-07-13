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

    /// <summary>
    /// Manejador de comandos para actualizar la contraseña de un usuario.
    /// </summary>
    /// <param name="request">Comando para actualizar la contraseña del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Respuesta que indica si la contraseña se actualizó correctamente.</returns>
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

    /// <summary>
    /// Método asincrónico que actualiza la contraseña de un usuario.
    /// </summary>
    /// <param name="request">Comando para actualizar la contraseña del usuario.</param>
    /// <returns>Respuesta que indica si la contraseña se actualizó correctamente.</returns>
    private async Task<UpdatePasswordResponse> HandleAsync(UpdatePasswordCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();

        try
        {
            _logger.LogInformation("UpdatePasswordCommandHandler.HandleAsync {Request}", request);

            switch (request.Request.UserType)
            {
                case "consumer":
                    var consumerEntity = _dbContext.Consumers.Find(request.Id);
                    if (consumerEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            consumerEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Consumers.Update(consumerEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {request.Id} not found");
                    }
                    break;
                case "provider":
                    var providerEntity = _dbContext.Providers.Find(request.Id);
                    if (providerEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            providerEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Providers.Update(providerEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {request.Id} not found");
                    }
                    break;
                case "admin":
                    var adminEntity = _dbContext.Admins.Find(request.Id);
                    if (adminEntity != null)
                    {
                        if (request.Request.PasswordHash != null)
                            adminEntity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                        _dbContext.Admins.Update(adminEntity);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Object with key {request.Id} not found");
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
            throw new CustomException(ex);
        }
    }
}