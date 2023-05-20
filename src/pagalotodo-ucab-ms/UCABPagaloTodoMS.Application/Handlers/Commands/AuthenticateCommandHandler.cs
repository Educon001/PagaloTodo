using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Infrastructure.Database;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Application.Handlers.Commands;

public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, LoginResponse>
{
    private readonly UCABPagaloTodoDbContext _dbContext;
    private readonly ILogger<AuthenticateCommandHandler> _logger;

    public AuthenticateCommandHandler(UCABPagaloTodoDbContext dbContext, ILogger<AuthenticateCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

   public async Task<LoginResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscamos el usuario en la tabla de administradores
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == request.Username);
            if (admin != null && SecurePasswordHasher.Verify(request.PasswordHash, admin.PasswordHash))
            {
                _logger.LogInformation($"El administrador {request.Username} inició sesión con éxito.");
                return new LoginResponse { UserType = "admin", Id = admin.Id };
            }

            // Buscamos el usuario en la tabla de consumidores
            var consumer = await _dbContext.Consumers.FirstOrDefaultAsync(c => c.Username == request.Username);
            if (consumer != null && SecurePasswordHasher.Verify(request.PasswordHash, consumer.PasswordHash))
            {
                _logger.LogInformation($"El consumidor {request.Username} inició sesión con éxito.");
                return new LoginResponse { UserType = "consumer", Id = consumer.Id };
            }

            // Buscamos el usuario en la tabla de proveedores
            var provider =await _dbContext.Providers.FirstOrDefaultAsync(p => p.Username == request.Username);
            if (provider != null && SecurePasswordHasher.Verify(request.PasswordHash, provider.PasswordHash))
            {
                _logger.LogInformation($"El proveedor {request.Username} inició sesión con éxito.");
                return new LoginResponse { UserType = "provider", Id = provider.Id };
            }

            _logger.LogWarning($"El usuario {request.Username} no pudo iniciar sesión porque no se encontró en la tabla de usuarios o la contraseña es incorrecta.");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al buscar al usuario {request.Username} en la base de datos.");
            throw new Exception($"No se pudo encontrar al usuario en la base de datos: {ex.Message}");
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al autenticar al usuario {request.Username}.");
            throw new Exception($"Ocurrió un error al autenticar al usuario: {ex.Message}");
        }
    }
}