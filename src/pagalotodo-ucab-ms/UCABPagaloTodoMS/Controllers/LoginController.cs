using System.IdentityModel.Tokens.Jwt;
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Authenticate(LoginRequest request)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(request));
            if (result == null)
            {
                return BadRequest();
            }

            string jwtToken = GenerateToken(result);

            return Ok(new LoginResponse {UserType = result.UserType, Id = result.Id, Token = jwtToken});
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al intentar registrar un consumidor. Exception: " + ex);
            return BadRequest(ex.Message + "\n" + ex.InnerException?.Message);
        }
    }

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
            var url = Url.Action("ResetPassword", "Login", new {token = token},
                protocol: HttpContext.Request.Scheme);
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

    [HttpPost("ResetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdatePasswordResponse>> ResetPassword([FromQuery] Guid token,
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

    private string GenerateToken(LoginResponse result)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("aA1$Bb2&Cc3^Dd4#Ee5!Ff6*Gg7(Hh8)Ii9Jj0");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new Claim("UserType", result.UserType)
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}