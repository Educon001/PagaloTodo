using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Core.Database;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class CreateConsumerCommandHandler : IRequestHandler<CreateConsumerCommand, Guid>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<CreateConsumerCommandHandler> _logger;

    public CreateConsumerCommandHandler(IUCABPagaloTodoDbContext dbContext,
        ILogger<CreateConsumerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Este método maneja una solicitud de inicio de sesión. Verifica que la solicitud no sea nula y llama al método HandleAsync para autenticar al usuario.
    /// Lanza una excepción de tipo ArgumentNullException si la solicitud es nula.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>Un objeto LoginResponse que contiene el tipo de usuario y el ID del usuario autenticado.</returns>
    public async Task<Guid> Handle(CreateConsumerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("CreateConsumerCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            var validator = new ConsumerRequestValidator();
            validator.ValidateAndThrow(request.Request);
            return await HandleAsync(request);
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Este método HandleAsync maneja la creación de un nuevo consumidor en la base de datos.
    /// Recibe un objeto CreateConsumerCommand que contiene la solicitud de creación del consumidor.
    /// Retorna un Guid que representa el id del consumidor creado.
    /// </summary>
    /// <param name="request">La solicitud de creación del consumidor.</param>
    /// <returns>Un Guid que representa el id del consumidor creado.</returns>
    private async Task<Guid> HandleAsync(CreateConsumerCommand request)
    {
        var transaccion = _dbContext.BeginTransaction();
        try
        {
            _logger.LogInformation("CreateConsumerCommandHandler.HandleAsync {Request}", request);
            var entity = ConsumerMapper.MapRequestToEntity(request.Request);
            _dbContext.Consumers.Add(entity);
            var id = entity.Id;
            await _dbContext.SaveEfContextChanges("APP");
            transaccion.Commit();
            _logger.LogInformation("CreateConsumerCommandHandler.HandleAsync {Response}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CreateConsumerCommandHandler.HandleAsync. {Mensaje}", ex.Message);
            transaccion.Rollback();
            throw new CustomException(ex);
        }
    }
}