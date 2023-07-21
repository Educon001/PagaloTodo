using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Payments;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, List<PaymentResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetPaymentsQueryHandler> _logger;

    public GetPaymentsQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetPaymentsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<PaymentResponse>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetPaymentsQueryHandler.Handle: Request nulo.");
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
    /// Handles the retrieval of payments from the database.
    /// </summary>
    /// <param name="request">The request containing optional date range.</param>
    /// <returns>A list of payments based on the specified date range.</returns>
    private async Task<List<PaymentResponse>> HandleAsync(GetPaymentsQuery request)
    {
        try
        {
            _logger.LogInformation("GetPaymentsQueryHandler.HandleAsync");
            var result = _dbContext.Payments.IgnoreQueryFilters().Where(p =>
                    p.CreatedAt >= (request.StartDate ?? DateTime.MinValue) &&
                    p.CreatedAt <= (request.EndDate ?? DateTime.MaxValue))
                .Include(p => p.Consumer)
                .Include(p => p.Service)
                    .ThenInclude(s=>s!.Provider)
                .Select(c => PaymentMapper.MapEntityToResponse(c, false, false));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetPaymentsQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}