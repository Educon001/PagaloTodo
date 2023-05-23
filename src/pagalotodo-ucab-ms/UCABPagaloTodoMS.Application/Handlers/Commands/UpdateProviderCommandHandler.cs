using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class UpdateProviderCommandHandler : IRequestHandler<UpdateProviderCommand, ProviderResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdateProviderCommandHandler> _logger;

    public UpdateProviderCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<UpdateProviderCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ProviderResponse> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("UpdateProviderCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            var validator = new ProviderRequestValidator();
            validator.ValidateAndThrow(request.Request);
            return await HandleAsync(request);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<ProviderResponse> HandleAsync(UpdateProviderCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdateProviderCommandHandler.HandleAsync {Request}", request);
            var providerId = request.Id;
            var entity = _dbContext.Providers.Find(providerId);
            if (entity!=null)
            {
                entity.Rif = request.Request.Rif;
                entity.AccountNumber = request.Request.AccountNumber;
                entity.Username = request.Request.Username;
                if (request.Request.PasswordHash!=null)
                {
                    entity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                }
                entity.Email = request.Request.Email;
                entity.Name = request.Request.Name;
                entity.LastName = request.Request.LastName;
                entity.Status = request.Request.Status;
                _dbContext.Providers.Update(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Object with key {providerId} not found");
            }
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            var response = ProviderMapper.MapEntityToResponse(entity);
            _logger.LogInformation("UpdateProviderCommandHandler.HandleAsync {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdateProviderCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}