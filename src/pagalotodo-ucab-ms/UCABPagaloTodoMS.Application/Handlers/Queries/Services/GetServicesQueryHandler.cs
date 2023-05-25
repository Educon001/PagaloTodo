using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, List<ServiceResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetServicesQueryHandler> _logger;

    public GetServicesQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetServicesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<ServiceResponse>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetServicesQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync();
        }
        catch (Exception)
        {
            _logger.LogWarning("GetProvidersQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<List<ServiceResponse>> HandleAsync()
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = _dbContext.Services.Include(s=>s.Provider).Select(c => ServiceMapper.MapEntityToResponse(c, false));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}