using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Queries.Consumers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Base;
using UCABPagaloTodoMS.Infrastructure.Utils;
using UCABPagaloTodoMS.Token;

namespace UCABPagaloTodoMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : BaseController<LoginController>
{
    private readonly IMediator _mediator;
    private IMemoryCache _cache;

    public LoginController(ILogger<LoginController> logger, IMediator mediator, IMemoryCache cache) : base(logger)
    {
        _mediator = mediator;
        _cache = cache;
    }

    /// <summary>
    /// Endpoint para la autenticación de usuarios.
    /// </summary>
    /// <remarks>
    /// ## Description
    /// Este método simula el inicio de sesión de un usuario y devuelve un token JWT válido
    /// si las credenciales son correctas.
    /// </remarks>
    /// <response code="200">
    /// Accepted:
    /// - Operación exitosa.
    /// </response>
    /// <response code="400">
    /// BadRequest:
    /// - Las credenciales proporcionadas son inválidas o incompletas.
    /// </response>
    /// <response code="401">
    /// Unauthorized:
    /// - Las credenciales proporcionadas no son válidas.
    /// </response>
    /// <returns>Retorna un objeto LoginResponse que contiene el tipo de usuario,
    /// el id y el token JWT generado para el usuario autenticado.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Authenticate(LoginRequest request)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(request));
            if (result == null)
            {
                return BadRequest();
            }
            
            string jwtToken = GenerarToken.GenerateToken(result);
            return Ok(new LoginResponse { UserType = result.UserType, Id = result.Id, Token = jwtToken });
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogError("La cuenta del usuario está inactiva. Exception: " + ex);
            return Unauthorized("La cuenta del usuario está inactiva. Exception: ");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Las credenciales de inicio de sesión son inválidas. Exception: " + ex);
            return BadRequest("Las credenciales de inicio de sesión son inválidas. Exception: ");
        }
        catch (CustomException ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al autenticar al usuario {request.Username}.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al autenticar al usuario {request.Username}.");
            return BadRequest("Se ha producido un error al autenticar al usuario.");
        }
    }

    /// <summary>
    /// Endpoint para enviar un correo electrónico al usuario con un enlace para restablecer su contraseña.
    /// </summary>
    /// <param name="email">El correo electrónico del usuario.</param>
    /// <returns>Retorna un objeto ForgotPasswordResponse que contiene un mensaje de éxito.</returns>
    /// <response code="200">Operación exitosa.</response>
    /// <response code="400">Se produjo un error al enviar el correo electrónico.</response>
    [HttpPost("ForgotPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] string email)
    {
        try
        {
            var query = new GetConsumerByEmailQuery(email);
            var queryResponse = await _mediator.Send(query);
            var token = Guid.NewGuid();
            // var url = Url.Action("ResetPassword", "Login", new {token = token},
            //     protocol: HttpContext.Request.Scheme);
            var url = $"https://localhost:7039/consumer/updatePassword/{token}";
            var urlInfo = new Dictionary<string, object>()
                {
                    {"UserId", queryResponse},
                    {"ExpirationDate", DateTime.UtcNow.AddMinutes(15)}
                }
                ;
            _cache.Set(token, urlInfo, TimeSpan.FromMinutes(15));
            var request = new ForgotPasswordRequest(email, url);
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("ForgotPassword Exception. Exception: " + ex);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Endpoint para resetear la contraseña del usuario.
    /// </summary>
    /// <param name="token">El token generado para el usuario.</param>
    /// <param name="request">La solicitud de cambio de contraseña.</param>
    /// <returns>Retorna un objeto UpdatePasswordResponse que contiene un mensaje de éxito.</returns>
    /// <response code="200">Operación exitosa.</response>
    /// <response code="400">Se produjo un error al procesar la solicitud de cambio de contraseña.</response>
    [HttpPost("ResetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdatePasswordResponse>> ResetPassword([FromQuery] Guid? token,
        [FromBody] UpdatePasswordRequest request)
    {
        try
        {
            var info = _cache.Get<Dictionary<string, object>>(token);
            if (info is null)
            {
                throw new CustomException($"token {token} not found",
                    new KeyNotFoundException(token.ToString()));
            }
            request.UserType = "consumer";
            var query = new UpdatePasswordCommand(request, (Guid) info["UserId"]);
            var response = await _mediator.Send(query);
            _cache.Remove(token);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("ResetPassword Exception. Exception: " + e);
            return BadRequest(e.Message);
        }
    }
}