using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Providers;

public class GetProvidersWithServicesQueryHandler : IRequestHandler<GetProvidersWithServicesQuery, List<ProviderResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetProvidersWithServicesQueryHandler> _logger;

    public GetProvidersWithServicesQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetProvidersWithServicesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<ProviderResponse>> Handle(GetProvidersWithServicesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetProvidersWithServicesQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return HandleAsync();
        }
        catch (Exception)
        {
            _logger.LogWarning("GetProvidersWithServicesQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<List<ProviderResponse>> HandleAsync()
    {
        try
        {
            _logger.LogInformation("GetProvidersWithServicesQueryHandler.HandleAsync");
            var result = _dbContext.Providers.Include(p=>p.Services)!
                .Select(p => ProviderMapper.MapEntityToResponse(p));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetProvidersWithServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}