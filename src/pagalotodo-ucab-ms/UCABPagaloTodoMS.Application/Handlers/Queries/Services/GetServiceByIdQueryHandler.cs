using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;

    public GetServiceByIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetServiceByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<ServiceResponse> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetServicesQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync(request);
        }
        catch (Exception)
        {
            _logger.LogWarning("GetProvidersQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<ServiceResponse> HandleAsync(GetServiceByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = await _dbContext.Services.Include(s=>s.Provider)
                .Include(s => s.ConciliationFormat)
                .Include(s => s.ConfirmationList)
                .Include(s => s.Payments)
                .SingleAsync(c => c.Id == request.Id);
            return ServiceMapper.MapEntityToResponse(result,false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}