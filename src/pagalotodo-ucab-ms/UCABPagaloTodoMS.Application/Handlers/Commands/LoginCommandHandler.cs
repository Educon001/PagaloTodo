using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Infrastructure.Database;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(UCABPagaloTodoDbContext dbContext, ILogger<LoginCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

   public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Request.Username != null && request.Request.PasswordHash != null &&
                request.Request.UserType != null)
            {
                // Buscamos el usuario en la tabla de administradores
                if (request.Request.UserType != null && request.Request.UserType.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (admin != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, admin.PasswordHash!))
                    {
                        _logger.LogInformation($"El administrador {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse {UserType = "admin", Id = admin.Id };
                    }
                    
                }
            
                // Buscamos el usuario en la tabla de consumidores
                if (request.Request.UserType != null && request.Request.UserType.Equals("CONSUMER", StringComparison.OrdinalIgnoreCase))
                {
                    var consumer = await _dbContext.Consumers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (consumer != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, consumer.PasswordHash!))
                    {
                        _logger.LogInformation($"El consumidor {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse {UserType = "consumer", Id = consumer.Id };
                    }
                    
                }

                // Buscamos el usuario en la tabla de proveedores
                if (request.Request.UserType != null && request.Request.UserType.Equals("PROVIDER", StringComparison.OrdinalIgnoreCase))
                {
                    var provider = await _dbContext.Providers.FirstOrDefaultAsync(a => a.Username == request.Request.Username);
                    if (provider != null && SecurePasswordHasher.Verify(request.Request.PasswordHash, provider.PasswordHash!))
                    {
                        _logger.LogInformation($"El proveedor {request.Request.Username} inició sesión con éxito.");
                        return new LoginResponse {UserType = "provider", Id = provider.Id };
                    }
                    
                }
                
                _logger.LogWarning($"El usuario {request.Request.Username} no pudo iniciar sesión porque no se encontró en la tabla de usuarios o la contraseña es incorrecta.");
                return null;
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }


        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al buscar al usuario {request.Request.Username} en la base de datos.");
            throw new Exception($"No se pudo encontrar al usuario en la base de datos: {ex.Message}");
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al autenticar al usuario {request.Request.Username}.");
            throw new Exception($"Ocurrió un error al autenticar al usuario: {ex.Message}");
        }
    }
}