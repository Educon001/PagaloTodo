using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoMS.Core.Entities;

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
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreatePaymentCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            var validator = new PaymentRequestValidator();
            validator.ValidateAndThrow(request.Request);
            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the creation of a payment in the database.
    /// </summary>
    /// <param name="request">The request containing the payment information.</param>
    /// <returns>The ID of the newly created payment.</returns>
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
            if (service.ServiceType == ServiceTypeEnum.PorConfirmacion)
            {
                var debtor = await _dbContext.Debtors.SingleAsync(d =>
                    d.Service == service && d.Identifier == request.Request.Identifier && d.Status==false);
                debtor.Status = true;
                _dbContext.Debtors.Update(debtor);
            }
            var entity = PaymentMapper.MapRequestToEntity(request.Request, service, consumer);
            entity.TransactionId = Guid.NewGuid().ToString();
            _dbContext.Payments.Add(entity);
            var id = entity.Id;
            if (request.Request.PaymentDetails is not null)
            {
                foreach (var detail in request.Request.PaymentDetails!)
                {
                    detail.Payment = id;
                    
                    //Verificar si el detalle esta en la entidad de PaymentFields
                    var field = await _dbContext.PaymentFields.SingleAsync(c =>
                        c.Service == entity.Service && c.Name == detail.Name);
                    if (field.Format is not null)
                    {
                        var validator = new PaymentDetailValidator(field.Format);
                        validator.ValidateAndThrow(detail);
                    }
                    var entityPDetail = PaymentDetailMapper.MapRequestToEntity(detail, entity);

                    //PaymentDetails entity add
                    _dbContext.PaymentDetails.Add(entityPDetail);
                }
            }
            
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
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