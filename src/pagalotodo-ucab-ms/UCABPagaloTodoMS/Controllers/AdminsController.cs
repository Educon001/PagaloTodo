using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Payments;
using UCABPagaloTodoMS.Application.Queries.Providers;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Authorization;
using UCABPagaloTodoMS.Base;
using UCABPagaloTodoMS.Core.Enums;

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

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("{id:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdatePasswordResponse>> UpdatePassword(Guid id,
        [FromBody] UpdatePasswordRequest request)
    {
        _logger.LogInformation("Entrando al método que cambia clave a admin");
        try
        {
            request.UserType = "admin";
            var query = new UpdatePasswordCommand(request, id);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al cambiar clave al admin. Exception: " + ex);
            return BadRequest(ex.Message);
        }
    }

    // [Authorize(Policy = "AdminPolicy" )]
    [HttpGet("Cierre")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> AccountingClose()
    {
        _logger.LogInformation("Entrando al método que ejecuta el cierre contable");
        try
        {
            var lastCloseQuery = new GetLastAccountingCloseQuery();
            var lastClose = await _mediator.Send(lastCloseQuery);
            var providersQuery = new GetProvidersWithServicesQuery();
            var providers = await _mediator.Send(providersQuery);
            foreach (var provider in providers)
            {
                if (provider.Services is not null)
                {
                    foreach (var service in provider.Services)
                    {
                        var fieldsQuery = new GetFieldsByServiceIdQuery(service.Id);
                        var fields = await _mediator.Send(fieldsQuery);
                        service.ConciliationFormat = fields;
                        var paymentsQuery = new GetPaymentsByServiceIdQuery(service.Id, lastClose, null);
                        var payments = await _mediator.Send(paymentsQuery);
                        service.Payments = payments.Where(p => p.PaymentStatus == PaymentStatusEnum.Pendiente).ToList();
                    }
                }
            }

            var request = new AccountingCloseRequest(providers);
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocurrio un error al ejecutar el cierre contable. Exception: " + ex);
            return BadRequest(ex.Message);
        }
    }
}