using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
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

    /// <summary>
    /// Handles the retrieval of all providers from the database, including their associated services.
    /// </summary>
    /// <returns>A list of all providers and their associated services retrieved from the database.</returns>
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
            throw new CustomException(ex);
        }
    }
}