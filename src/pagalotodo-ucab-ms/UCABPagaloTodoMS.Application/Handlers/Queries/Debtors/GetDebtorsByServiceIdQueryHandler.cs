using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Debtors;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetDebtorsByServiceIdQueryHandler : IRequestHandler<GetDebtorsByServiceIdQuery, List<DebtorsResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetDebtorsByServiceIdQueryHandler> _logger;

    public GetDebtorsByServiceIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetDebtorsByServiceIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<DebtorsResponse>> Handle(GetDebtorsByServiceIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetDebtorsByServiceIdQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync(request);
        }
        catch (Exception)
        {
            _logger.LogWarning("DebtorsByServiceIdQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<List<DebtorsResponse>> HandleAsync(GetDebtorsByServiceIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetDebtorsByServiceIdQueryHandler.HandleAsync");
            var result = _dbContext.Debtors.Where(c=>c.Service!.Id == request.Id).Select(c => DebtorsMapper.MapEntityToResponse(c));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetDebtorsByServiceIdQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}