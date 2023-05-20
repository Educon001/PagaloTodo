using MediatR;
using Microsoft.AspNetCore.Mvc;
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

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al intentar registrar un consumidor. Exception: " + ex);
            return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
        }
    }
}