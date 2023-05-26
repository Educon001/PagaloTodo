using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Services;

public class GetServicesByProviderIdQueryHandler : IRequestHandler<GetServicesByProviderIdQuery, List<ServiceResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetServicesByProviderIdQueryHandler> _logger;

    public GetServicesByProviderIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetServicesByProviderIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<ServiceResponse>> Handle(GetServicesByProviderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetServicesByProviderIdQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync(request);
        }
        catch (Exception)
        {
            _logger.LogWarning("GetServicesByProviderIdQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<List<ServiceResponse>> HandleAsync(GetServicesByProviderIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesByProviderIdQueryHandler.HandleAsync");
            var result = _dbContext.Services.Where(c=>c.Provider!.Id == request.Id)
                .Include(s=>s.Payments)!
                    .ThenInclude(p=>p.Consumer)
                .Include(s=>s.ConciliationFormat)
                .Include(s=>s.ConfirmationList)
                .Select(c => ServiceMapper.MapEntityToResponse(c,true));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetServicesByProviderIdQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}