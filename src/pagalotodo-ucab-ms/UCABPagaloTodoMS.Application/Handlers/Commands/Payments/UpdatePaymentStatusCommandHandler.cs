using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands.Payments;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands.Payments;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, string>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<UpdatePaymentStatusCommandHandler> _logger;

    public UpdatePaymentStatusCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<UpdatePaymentStatusCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty || request.NewPaymentStatus == null)
            {
                _logger.LogWarning("UpdatePaymentStatusCommandHandler.Handle: Request nulo.");
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
    /// Handles the update of a payment's status in the database.
    /// </summary>
    /// <param name="request">The request containing the payment ID and new payment status.</param>
    /// <returns>A message indicating the success of the update.</returns>
    private async Task<string> HandleAsync(UpdatePaymentStatusCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("UpdatePaymentStatusCommandHandler.HandleAsync {Request}", request);
            var entity = _dbContext.Payments.Find(request.Id);
            if (entity != null)
            {
                entity.PaymentStatus = request.NewPaymentStatus;
                _dbContext.Payments.Update(entity);
            }
            else
            {
                throw new KeyNotFoundException($"Object with key {request.Id} not found");
            }

            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            var response = request.Id + " PaymentStatus updated successfully";
            _logger.LogInformation("UpdatePaymentStatusCommandHandler.HandleAsync {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error UpdatePaymentStatusCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw;
        }
    }
}