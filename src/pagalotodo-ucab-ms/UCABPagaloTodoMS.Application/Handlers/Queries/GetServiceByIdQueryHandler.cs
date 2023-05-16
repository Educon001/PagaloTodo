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
            _logger.LogWarning("ProvidersQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<ServiceResponse> HandleAsync(GetServiceByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = _dbContext.Services.Where(c=>c.Id == request.Id && c.IsDeleted==false).Select(c => ServiceMapper.MapEntityToResponse(c));
            return (await result.FirstOrDefaultAsync())!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}