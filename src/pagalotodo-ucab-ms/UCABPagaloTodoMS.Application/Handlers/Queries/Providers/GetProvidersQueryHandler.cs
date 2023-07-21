using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Providers;

public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, List<ProviderResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetProvidersQueryHandler> _logger;

    public GetProvidersQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetProvidersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<ProviderResponse>> Handle(GetProvidersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetProvidersQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return HandleAsync();
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the retrieval of all providers from the database.
    /// </summary>
    /// <returns>A list of all providers retrieved from the database.</returns>
    private async Task<List<ProviderResponse>> HandleAsync()
    {
        try
        {
            _logger.LogInformation("GetProvidersQueryHandler.HandleAsync");
            var result = _dbContext.Providers.Select(p => ProviderMapper.MapEntityToResponse(p));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetProvidersQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}