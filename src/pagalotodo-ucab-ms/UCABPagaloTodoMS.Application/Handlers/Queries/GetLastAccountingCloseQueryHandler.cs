using Castle.Core.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Queries;

public class GetLastAccountingCloseQueryHandler : IRequestHandler<GetLastAccountingCloseQuery, DateTime>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<GetLastAccountingCloseQueryHandler> _logger;

    public GetLastAccountingCloseQueryHandler(IUCABPagaloTodoDbContext dbContext, ILogger<GetLastAccountingCloseQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<DateTime> Handle(GetLastAccountingCloseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleAsync();
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Handles the retrieval of the date and time of the last executed accounting closure from the database.
    /// </summary>
    /// <returns>The date and time of the last executed accounting closure.</returns>
    private async Task<DateTime> HandleAsync()
    {
        try
        {
            _logger.LogInformation("GetLastAccountingCloseQueryHandler.HandleAsync");

            var result = _dbContext.AccountingClosures.Any()?
                await _dbContext.AccountingClosures.MaxAsync(e => e.ExecutedAt) : DateTime.MinValue;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GetLastAccountingCloseQueryHandler.HandleAsync. {Mensaje}", ex.Message);
            throw;
        }
    }
}