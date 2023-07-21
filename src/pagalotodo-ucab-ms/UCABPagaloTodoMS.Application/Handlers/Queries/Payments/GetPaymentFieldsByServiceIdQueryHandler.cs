using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Payments;

public class GetPaymentFieldsByServiceIdQueryHandler : IRequestHandler<GetPaymentFieldsByServiceIdQuery, List<PaymentFieldResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetPaymentFieldsByServiceIdQueryHandler> _logger;

    public GetPaymentFieldsByServiceIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetPaymentFieldsByServiceIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<PaymentFieldResponse>> Handle(GetPaymentFieldsByServiceIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetPaymentsByConsumerIdQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the retrieval of payment fields from the database by service ID.
    /// </summary>
    /// <param name="request">The request containing the ID of the service.</param>
    /// <returns>A list of payment fields associated with the specified service ID.</returns>
    private async Task<List<PaymentFieldResponse>> HandleAsync(GetPaymentFieldsByServiceIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetPaymentsByConsumerIdQueryHandler.HandleAsync");
            var result = _dbContext.PaymentFields.IgnoreQueryFilters().Where(p =>
                    p.Service!.Id == request.Id)
                .Select(c => PaymentFieldMapper.MapEntityToResponse(c));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetPaymentsByConsumerIdQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}