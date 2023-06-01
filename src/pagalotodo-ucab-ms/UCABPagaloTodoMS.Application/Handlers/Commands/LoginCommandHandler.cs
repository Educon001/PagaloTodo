using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Database;
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

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
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
                    var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (admin != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, admin.PasswordHash!))
                    {
                        _logger.LogInformation($"El administrador {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse {UserType = "admin", Id = admin.Id };
                    }
                    break;

                case "CONSUMER":
                    var consumer = await _dbContext.Consumers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (consumer != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, consumer.PasswordHash!))
                    {
                        _logger.LogInformation($"El consumidor {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse{
                            UserType= "consumer",
                            Id = consumer.Id
                        };
                    }
                    break;

                case "PROVIDER":
                    var provider = await _dbContext.Providers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (provider != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, provider.PasswordHash!))
                    {
                        _logger.LogInformation($"El proveedor {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse
                        {
                            UserType = "provider",
                            Id = provider.Id
                        };
                    }
                    break;

                default:
                    _logger.LogWarning($"El tipo de usuario {request.Request.UserType} no es válido.");
                    break;
            }

            // Si no se encuentra al usuario en ninguna de las tablas o la contraseña es incorrecta, devolver nulo
            _logger.LogWarning($"El usuario {request.Request.Username} no pudo iniciar sesión porque no se encontró en la tabla de usuarios o la contraseña es incorrecta.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al autenticar al usuario {request.Request.Username}.");
            throw new CustomException($"Ocurrió un error al autenticar al usuario: {ex.Message}", ex);
        }
    }
}