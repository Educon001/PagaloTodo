using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Services;

public class CreatePaymentFieldCommandHandler : IRequestHandler<CreatePaymentFieldCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreatePaymentFieldCommandHandler> _logger;

    public CreatePaymentFieldCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<CreatePaymentFieldCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreatePaymentFieldCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreatePaymentFieldCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    private async Task<Guid> HandleAsync(CreatePaymentFieldCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            if (_dbContext.Services.Find(request.Request.Service) is not null)
            {
                _logger.LogInformation("CreatePaymentFieldCommandHandler.HandleAsync {Request}", request);
                ServiceEntity? serviceE = _dbContext.Services.Find(request.Request.Service);
                var entity = PaymentFieldMapper.MapRequestToEntity(request.Request, serviceE!);

                //PaymentFields entity add
                _dbContext.PaymentFields.Add(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                _logger.LogInformation("CreatePaymentFieldCommandHandler.HandleAsync {Response}", entity.Id);
                return entity.Id;
            }

            throw new Exception($"Servicio con id: {request.Request.Service} no se encuentra en la base de datos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreatePaymentFieldCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}