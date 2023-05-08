using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetServicesByIdQueryHandler : IRequestHandler<GetServicesByIdQuery, ServiceResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetServicesQueryHandler> _logger;

    public GetServicesByIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetServicesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<ServiceResponse> Handle(GetServicesByIdQuery request, CancellationToken cancellationToken)
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
            _logger.LogWarning("ProvidersQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<ServiceResponse> HandleAsync(GetServicesByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = _dbContext.Services.Where(c=>c.Id == request.Id).Select(c => ServiceMapper.MapEntityToResponse(c));
            return (await result.FirstOrDefaultAsync())!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}