using UCABPagaloTodoMS.Core.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Application.Mappers;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class ProvidersQueryHandler : IRequestHandler<ProvidersQuery, List<ProviderResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<ProvidersQueryHandler> _logger;

    public ProvidersQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<ProvidersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<ProviderResponse>> Handle(ProvidersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("ProvidersQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                return HandleAsync();
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("ProvidersQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<List<ProviderResponse>> HandleAsync()
    {
        try
        {
            _logger.LogInformation("ProvidersQueryHandler.HandleAsync");

            var result = _dbContext.Providers.Select(c => ProviderMapper.MapEntityToResponse(c));

            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ProvidersQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}