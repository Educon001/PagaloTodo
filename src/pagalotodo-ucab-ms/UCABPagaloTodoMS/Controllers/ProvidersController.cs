using MediatR;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Base;

namespace UCABPagaloTodoMS.Controllers;

[ApiController]
[Route("[controller]")]
public class ProvidersController : BaseController<ProvidersController>
    {
        private readonly IMediator _mediator;

        public ProvidersController(ILogger<ProvidersController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Endpoint para la consulta de proveedores
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get proveedores
        ///     ## Url
        ///     GET /providers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de proveedores.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProviderResponse>>> GetProviders()
        {
            _logger.LogInformation("Entrando al método que consulta los proveedores");
            try
            {
                var query = new ProvidersQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los proveedores. Exception: " + ex);
                return BadRequest(ex);
            }
        }

        /// <summary>
        ///     Endpoint que registra un proveedor.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post proveedor.
        ///     ## Url
        ///     POST /providers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del nuevo registro.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> PostProvider(ProviderRequest valor)
        {
            _logger.LogInformation("Entrando al método que registra los proveedores");
            try
            {
                var query = new CreateProviderCommand(valor);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un proveedor. Exception: " + ex);
                return BadRequest(ex);
            }
        }

}