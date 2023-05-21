using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Base;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : BaseController<AuthenticationController>
{
    private readonly IMediator _mediator;

    public AuthenticationController(ILogger<AuthenticationController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Authenticate([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _mediator.Send(new AuthenticateCommand { Username = request.Username, PasswordHash = request.PasswordHash });
            if (result == null)
            {
                return BadRequest();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("aA1$Bb2&Cc3^Dd4#Ee5!Ff6*Gg7(Hh8)Ii9Jj0");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, result.Id.ToString()),
                    new Claim(ClaimTypes.Role, result.UserType)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new LoginResponse { UserType = result.UserType, Id = result.Id, Token = tokenString });
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al intentar registrar un consumidor. Exception: " + ex);
            return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
        }
    }
}