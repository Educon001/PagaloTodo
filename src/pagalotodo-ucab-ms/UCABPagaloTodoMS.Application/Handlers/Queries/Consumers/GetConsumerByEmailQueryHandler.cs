using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Consumers;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Consumers;

public class GetConsumerByEmailQueryHandler : IRequestHandler<GetConsumerByEmailQuery, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetConsumerByEmailQueryHandler> _logger;

    public GetConsumerByEmailQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetConsumerByEmailQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> Handle(GetConsumerByEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Email is null || request.Email.Equals(default))
            {
                _logger.LogWarning("GetConsumerByEmailQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogWarning("GetConsumerByEmailQueryHandler.Handle: ArgumentNullException");
            throw new CustomException(e);
        }
    }

    private async Task<Guid> HandleAsync(GetConsumerByEmailQuery request)
    {
        try
        {
            _logger.LogInformation("GetConsumerByEmailQueryHandler.HandleAsync");
            var result = await _dbContext.Consumers
                .SingleAsync(p => p.Email == request.Email);
            return result.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetConsumerByEmailQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}