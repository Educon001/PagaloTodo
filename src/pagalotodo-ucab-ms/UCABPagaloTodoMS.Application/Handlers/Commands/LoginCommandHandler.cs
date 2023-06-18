using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUCABPagaloTodoDbContext dbContext, ILogger<LoginCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// Maneja una solicitud de inicio de sesión. Verifica que la solicitud no sea nula y llama al método HandleAsync para autenticar al usuario.
    /// Lanza una excepción de tipo ArgumentNullException si la solicitud es nula.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>Un objeto LoginResponse que contiene el tipo de usuario y el ID del usuario autenticado.</returns>
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request == null)
            {
                _logger.LogWarning("LoginCommandHandler.Handle: Request nulo.");
                throw new ArgumentNullException(nameof(request));
            }
            return await HandleAsync(request);
        }
        catch (HttpRequestException)
        {
            throw; // Si ya es una excepción de tipo HttpRequestException, simplemente relanzarla
        }
        catch (ArgumentNullException)
        {
            throw; // Si ya es una excepción de tipo ArgumentNullException, simplemente relanzarla
        }
        catch (Exception e)
        {
            throw new CustomException(e);
        }
    }

    /// <summary>
    /// Autentica al usuario y devuelve un objeto LoginResponse que contiene el tipo de usuario y el ID del usuario autenticado.
    /// Lanza una excepción de tipo HttpRequestException con un mensaje de error y un código de estado HTTP si la cuenta del usuario está inactiva o las credenciales son inválidas.
    /// Lanza una excepción de tipo CustomException si se produce algún otro tipo de error y lo registra en el log de la aplicación.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión.</param>
    /// <returns>Un objeto LoginResponse que contiene el tipo de usuario y el ID del usuario autenticado.</returns>
    private async Task<LoginResponse> HandleAsync(LoginCommand request)
    {
        try
        {
            // Verificar que se proporcione el nombre de usuario, la contraseña y el tipo de usuario en la solicitud
            if (request.Request.Username == null || request.Request.PasswordHash == null || request.Request.UserType == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // Buscar al usuario en la tabla correspondiente según el tipo de usuario proporcionado en la solicitud
            switch (request.Request.UserType.ToUpper())
            {
                case "ADMIN":
                    var admin = await _dbContext.Admins.FirstOrDefaultAsync(a =>
                        a.Username == request.Request.Username);
                    if (admin != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, admin.PasswordHash!))
                    {
                        if (admin.Status == true)
                        {
                            _logger.LogInformation(
                                $"El administrador {request.Request.Username} inició sesión con éxito.");
                            return new LoginResponse { UserType = "admin", Id = admin.Id };
                        }
                        _logger.LogInformation($"La cuenta del administrador {request.Request.Username} está inactiva");
                        throw new HttpRequestException("La cuenta del administrador está inactiva.", null,
                            System.Net.HttpStatusCode.Unauthorized);
                    }

                    _logger.LogInformation(
                        $"El usuario {request.Request.Username} no pudo iniciar sesión porque las credenciales son inválidas.");
                    throw new HttpRequestException("Credenciales inválidas.", null,
                        System.Net.HttpStatusCode.BadRequest);

                case "CONSUMER":
                    var consumer =
                        await _dbContext.Consumers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (consumer != null &&
                        SecurePasswordHasher.Verify(request.Request.PasswordHash, consumer.PasswordHash!))
                    {
                        if (consumer.Status == true)
                        {
                            _logger.LogInformation(
                                $"El consumidor {request.Request.Username} inició sesión con éxito.");
                            return new LoginResponse { UserType = "consumer", Id = consumer.Id };
                        }

                        _logger.LogInformation($"La cuenta del consumidor {request.Request.Username} está inactiva");
                        throw new HttpRequestException("La cuenta del consumidor está inactiva.", null,
                            System.Net.HttpStatusCode.Unauthorized);
                    }

                    _logger.LogInformation(
                        $"El usuario {request.Request.Username} no pudo iniciar sesión porque las credenciales son inválidas.");
                    throw new HttpRequestException("Credenciales inválidas.", null,
                        System.Net.HttpStatusCode.BadRequest);

                case "PROVIDER":
                    var provider =
                        await _dbContext.Providers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (provider != null &&
                        SecurePasswordHasher.Verify(request.Request.PasswordHash, provider.PasswordHash!))
                    {
                        if (provider.Status == true)
                        {
                            _logger.LogInformation($"El proveedor {request.Request.Username} inició sesión con éxito.");
                            return new LoginResponse { UserType = "provider", Id = provider.Id };
                        }

                        _logger.LogInformation($"La cuenta del proveedor {request.Request.Username} está inactiva");
                        throw new HttpRequestException("La cuenta del proveedor está inactiva.", null,
                            System.Net.HttpStatusCode.Unauthorized);
                    }

                    _logger.LogInformation(
                        $"El usuario {request.Request.Username} no pudo iniciar sesión porque las credenciales son inválidas.");
                    throw new HttpRequestException("Credenciales inválidas.", null,
                        System.Net.HttpStatusCode.BadRequest);

                default:
                    _logger.LogWarning($"El tipo de usuario {request.Request.UserType} no es válido.");
                    break;
            }

            // Si no se encuentra al usuario en ninguna de las tablas o la contraseña es incorrecta, devolver nulo
            _logger.LogWarning(
                $"El usuario {request.Request.Username} no pudo iniciar sesión porque no se encontró en la tabla de usuarios o la contraseña es incorrecta.");
            return null;
        }
        catch (HttpRequestException)
        {
            throw; // Si ya es una excepción de tipo HttpRequestException, simplemente relanzarla
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió unerror al autenticar al usuario {request.Request.Username}.");
            throw new CustomException($"Ocurrió un error al autenticar al usuario: {ex.Message}", ex);
        }        
    }

}