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
        ///     Endpoint para la consulta de prestadores
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get prestadores
        ///     ## Url
        ///     GET /providers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de prestadores.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProviderResponse>>> GetProviders()
        {
            _logger.LogInformation("Entrando al método que consulta los prestadores");
            try
            {
                var query = new ProvidersQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los prestadores. Exception: " + ex);
                return BadRequest(ex);
            }
        }

        /// <summary>
        ///     Endpoint que registra un prestador.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post prestador.
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
        public async Task<ActionResult<Guid>> PostProvider(ProviderRequest provider)
        {
            _logger.LogInformation("Entrando al método que registra los prestadores");
            try
            {
                var query = new CreateProviderCommand(provider);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un prestador. Exception: " + ex);
                return BadRequest(ex);
            }
        }
        
        /// <summary>
        ///     Endpoint que elimina un prestador.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Eliminar prestador.
        ///     ## Url
        ///     DELETE /providers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el objeto eliminado</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteProvider(Guid id)
        {
            _logger.LogInformation("Entrando al método que elimina un prestador");
            try
            {
                var query = new DeleteProviderCommand(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un prestador. Exception: " + ex);
                return BadRequest(ex);
            }
        }

}