using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class ConsultarServiciosQueryHandler : IRequestHandler<ConsultarServiciosQuery, List<ServiceResponse>>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<ConsultarServiciosQueryHandler> _logger;

    public ConsultarServiciosQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<ConsultarServiciosQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<ServiceResponse>> Handle(ConsultarServiciosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("ConsultarServiciosQueryHandler.Handle: Request nulo.");
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

    private async Task<List<ServiceResponse>> HandleAsync()
    {
        try
        {
            _logger.LogInformation("ConsultarServiciosQueryHandler.HandleAsync");
            var result = _dbContext.Services.Select(c => ServiceMapper.MapEntityToResponse(c));
            return await result.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ServicesQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}