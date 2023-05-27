using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Payments;

public class GetPaymentsByServiceIdQueryHandler : IRequestHandler<GetPaymentsByServiceIdQuery, List<PaymentResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetPaymentsByServiceIdQueryHandler> _logger;

    public GetPaymentsByServiceIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetPaymentsByServiceIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<PaymentResponse>> Handle(GetPaymentsByServiceIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetServicesQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    private async Task<List<PaymentResponse>> HandleAsync(GetPaymentsByServiceIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = _dbContext.Payments.Where(p =>
                    p.Service!.Id == request.Id &&
                    p.CreatedAt >= (request.StartDate ?? DateTime.MinValue) &&
                    p.CreatedAt <= (request.EndDate ?? DateTime.MaxValue))
                .Include(p => p.Consumer)
                .Select(c => PaymentMapper.MapEntityToResponse(c, true, false));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}