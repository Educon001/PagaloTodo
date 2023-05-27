using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Consumers;

public class GetConsumerByIdQueryHandler : IRequestHandler<GetConsumerByIdQuery, ConsumerResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetConsumerByIdQueryHandler> _logger;

    public GetConsumerByIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetConsumerByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<ConsumerResponse> Handle(GetConsumerByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetConsumerByIdQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }

            return HandleAsync(request);
        }
        catch (Exception)
        {
            _logger.LogWarning("GetConsumerByIdQueryHandler.Handle: ArgumentNullException");
            throw;
        }
    }

    private async Task<ConsumerResponse> HandleAsync(GetConsumerByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetConsumerByIdQueryHandler.HandleAsync");
            var result = await _dbContext.Consumers
                .SingleAsync(p => p.Id == request.Id);
            return ConsumerMapper.MapEntityToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetConsumerByIdQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}