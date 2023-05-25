using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetFieldsByServiceIdQueryHandler : IRequestHandler<GetFieldsByServiceIdQuery, List<FieldResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetFieldsByServiceIdQueryHandler> _logger;

    public GetFieldsByServiceIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetFieldsByServiceIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<FieldResponse>> Handle(GetFieldsByServiceIdQuery request, CancellationToken cancellationToken)
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

    private async Task<List<FieldResponse>> HandleAsync(GetFieldsByServiceIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetServicesQueryHandler.HandleAsync");
            var result = _dbContext.Fields.Where(c=>c.Service!.Id == request.Id).Select(c => FieldMapper.MapEntityToResponse(c));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}