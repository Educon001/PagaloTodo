using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Core.Database;
using MediatR;
using Microsoft.Extensions.Logging;

namespace UCABPagaloTodoMS.Application.Handlers.Commands
{
    public class AgregarValorPruebaHandler : IRequestHandler<AgregarValorPruebaCommand, List<string>>
    {
        private readonly IUCABPagaloTodoDbContext _dbContext;
        private readonly ILogger<AgregarValorPruebaHandler> _logger;

        public AgregarValorPruebaHandler(IUCABPagaloTodoDbContext dbContext,ILogger<AgregarValorPruebaHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task<List<string>> Handle(AgregarValorPruebaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                {
                    _logger.LogWarning("AgregarValorPruebaHandler.Handle: Request nulo..");
                    throw new ArgumentNullException(nameof(request));
                }
                else
                {
                    return HandleAsync(request);
                }
            }
            catch (Exception)
            {
                _logger.LogWarning("AgregarValorPruebaHandler.Handle: ArgumentNullException");
                throw;
            }
        }

        private Task<List<string>> HandleAsync(AgregarValorPruebaCommand request)
        {
            using var transaction = _dbContext.BeginTransaction();
            try
            {
                _logger.LogInformation("AgregarValorPruebaHandler.HandleAsync", request.valor);
                /*await ValidateRules(request);
                var taxpayer = TaxPayerMapper.MapperRequestToEntity(request.Object);
                _context.Billings.Add(taxpayer);
                var billingId = taxpayer.Id;
                await _context.SaveEfContextChanges(_appSettings?.Value?.ApiUserName ?? string.Empty);
                transaction.Commit();
                return billingId;*/
                return Task.FromResult("".Split(",").ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error AgregarValorPruebaHandler.HandleAsync. {Mensaje}", ex.Message);
                transaction?.Rollback();
                throw;
            }
        }
    }
}
