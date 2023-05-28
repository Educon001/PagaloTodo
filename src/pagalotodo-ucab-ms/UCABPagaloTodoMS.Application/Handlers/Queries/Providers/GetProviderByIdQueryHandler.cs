using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Providers;

public class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, ProviderResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetProviderByIdQueryHandler> _logger;

    public GetProviderByIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetProviderByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<ProviderResponse> Handle(GetProviderByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetProviderByIdQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return HandleAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogWarning("GetProviderByIdQueryHandler.Handle: ArgumentNullException");
            throw new CustomException(e);
        }
    }

    private async Task<ProviderResponse> HandleAsync(GetProviderByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetProviderByIdQueryHandler.HandleAsync");
            var result = await _dbContext.Providers
                .Include(p => p.Services!)
                .SingleAsync(p => p.Id == request.Id);
            return ProviderMapper.MapEntityToResponse(result);
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }
    }
}