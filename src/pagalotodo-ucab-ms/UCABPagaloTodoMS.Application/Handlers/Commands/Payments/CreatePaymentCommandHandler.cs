using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Payments;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<CreatePaymentCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.Request == null)
        {
            _logger.LogWarning("CreatePaymentCommandHandler.Handle: Request nulo.");
            throw new ArgumentNullException(nameof(request));
        }

        return await HandleAsync(request);
    }

    private async Task<Guid> HandleAsync(CreatePaymentCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("CreatePaymentCommandHandler.HandleAsync {Request}", request);
            var service = _dbContext.Services.Find(request.Request.Service);
            var consumer = _dbContext.Consumers.Find(request.Request.Consumer);
            if (service is null || consumer is null)
            {
                var message = (service == null
                    ? $"-- Servicio con id: {request.Request.Service} no se encuentra en la base de datos" + "\n"
                    : "") + (consumer == null
                    ? $"-- Consumidor con id: {request.Request.Consumer} no se encuentra en la base de datos"
                    : "");
                throw new KeyNotFoundException(message);
            }
            var entity = PaymentMapper.MapRequestToEntity(request.Request, service, consumer);
            _dbContext.Payments.Add(entity);
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            var id = entity.Id;
            _logger.LogInformation("CreatePaymentCommandHandler.HandleAsync {Response}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreatePaymentCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}