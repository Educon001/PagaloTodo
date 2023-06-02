using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Payments;

public class GetPaymentsByConsumerIdQueryHandler : IRequestHandler<GetPaymentsByConsumerIdQuery, List<PaymentResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetPaymentsByConsumerIdQueryHandler> _logger;

    public GetPaymentsByConsumerIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetPaymentsByConsumerIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<PaymentResponse>> Handle(GetPaymentsByConsumerIdQuery request, CancellationToken cancellationToken)
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

    private async Task<List<PaymentResponse>> HandleAsync(GetPaymentsByConsumerIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetPaymentsByConsumerIdQueryHandler.HandleAsync");
            var result = _dbContext.Payments.IgnoreQueryFilters().Where(p =>
                    p.Consumer!.Id == request.Id &&
                    p.CreatedAt >= (request.StartDate ?? DateTime.MinValue) &&
                    p.CreatedAt <= (request.EndDate ?? DateTime.MaxValue))
                .Include(p => p.Service)
                    .ThenInclude(s=>s!.Provider)
                .Select(c => PaymentMapper.MapEntityToResponse(c, false, true));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetPaymentsByConsumerIdQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}