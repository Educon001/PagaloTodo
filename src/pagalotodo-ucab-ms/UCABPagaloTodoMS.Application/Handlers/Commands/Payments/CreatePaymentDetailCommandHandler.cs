using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Payments;

public class CreatePaymentDetailCommandHandler : IRequestHandler<CreatePaymentDetailCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreatePaymentDetailCommandHandler> _logger;

    public CreatePaymentDetailCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<CreatePaymentDetailCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreatePaymentDetailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreatePaymentDetailCommandHandler.Handle: Request nulo.");
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
    /// Handles the creation of a payment detail in the database.
    /// </summary>
    /// <param name="request">The request containing the payment detail information.</param>
    /// <returns>The ID of the newly created payment detail.</returns>
    private async Task<Guid> HandleAsync(CreatePaymentDetailCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            PaymentEntity? paymentE = _dbContext.Payments.Find(request.Request.Payment);
            if (paymentE is not null)
            {
                _logger.LogInformation("CreatePaymentDetailCommandHandler.HandleAsync {Request}", request);
                var entity = PaymentDetailMapper.MapRequestToEntity(request.Request, paymentE);
                
                //PaymentDetails entity add
                _dbContext.PaymentDetails.Add(entity);
                await _dbContext.SaveEfContextChanges("APP");
                transaccion.Commit();
                _logger.LogInformation("CreatePaymentDetailCommandHandler.HandleAsync {Response}", entity.Id);
                return entity.Id;
            }

            throw new Exception($"Servicio con id: {request.Request.Payment} no se encuentra en la base de datos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreatePaymentDetailCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}