using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries.Services;

public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FieldResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetFieldByIdQueryHandler> _logger;

    public GetFieldByIdQueryHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<GetFieldByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<FieldResponse> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                _logger.LogWarning("GetFieldsQueryHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the retrieval of a field from the database by ID.
    /// </summary>
    /// <param name="request">The request containing the ID of the field to retrieve.</param>
    /// <returns>The field information retrieved from the database.</returns>
    private async Task<FieldResponse> HandleAsync(GetFieldByIdQuery request)
    {
        try
        {
            _logger.LogInformation("GetFieldsQueryHandler.HandleAsync");
            var result = await _dbContext.Fields.SingleAsync(c => c.Id == request.Id);
            return FieldMapper.MapEntityToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error FieldsQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}