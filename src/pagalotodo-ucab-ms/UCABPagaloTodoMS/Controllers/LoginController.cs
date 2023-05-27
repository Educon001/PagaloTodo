using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
public class LoginController : BaseController<LoginController>
{
    private readonly IMediator _mediator;
    private IConfiguration config;

    public LoginController(ILogger<LoginController> logger, IMediator mediator, IConfiguration config) : base(logger)
    {
        _mediator = mediator;
        this.config = config;
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
            
            return Ok(new LoginResponse { UserType =result.UserType ,Id = result.Id, Token = jwtToken });
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al intentar registrar un consumidor. Exception: " + ex);
            return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
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
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
    
}