using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Authorization;
using UCABPagaloTodoMS.Base;

namespace UCABPagaloTodoMS.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminsController : BaseController<AdminsController>
{
    private readonly IMediator _mediator;

    public AdminsController(ILogger<AdminsController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }
    
    [Authorize(Policy = "AdminPolicy" )]
    [HttpPut("{id:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdatePasswordResponse>> UpdatePassword(Guid id, [FromBody]UpdatePasswordRequest request)
    {
        _logger.LogInformation("Entrando al método que cambia clave a admin");
        try
        {
            request.UserType = "admin";
            var query = new UpdatePasswordCommand(request,id);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al cambiar clave al admin. Exception: " + ex);
            return BadRequest(ex.Message);
        }
    }
    
    
}