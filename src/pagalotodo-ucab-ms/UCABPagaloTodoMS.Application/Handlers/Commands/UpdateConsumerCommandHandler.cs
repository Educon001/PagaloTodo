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

public class UpdateConsumerCommandHandler : IRequestHandler<UpdateConsumerCommand, ConsumerResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdateConsumerCommandHandler> _logger;

    public UpdateConsumerCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<UpdateConsumerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ConsumerResponse> Handle(UpdateConsumerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("UpdateConsumerCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                var validator = new ConsumerRequestValidator();
                validator.ValidateAndThrow(request.Request);
                return await HandleAsync(request);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<ConsumerResponse> HandleAsync(UpdateConsumerCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdateConsumerCommandHandler.HandleAsync {Request}", request);
            var consumerId = request.Id;
            var entity = _dbContext.Consumers.Find(consumerId);
            if (entity!=null)
            {
                entity.ConsumerId= request.Request.ConsumerId;
                entity.Username = request.Request.Username;
                if (request.Request.PasswordHash!=null)
                {
                    entity.PasswordHash = SecurePasswordHasher.Hash(request.Request.PasswordHash);
                }
                entity.Email = request.Request.Email;
                entity.Name = request.Request.Name;
                entity.LastName = request.Request.LastName;
                entity.Status = request.Request.Status;
                _dbContext.Consumers.Update(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Object with key {consumerId} not found");
            }
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            var response = ConsumerMapper.MapEntityToResponse(entity);
            _logger.LogInformation("UpdateConsumerCommandHandler.HandleAsync {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdateConsumerCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}