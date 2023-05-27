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
public class ConsumersController : BaseController<ConsumersController>
    {
        private readonly IMediator _mediator;

        public ConsumersController(ILogger<ConsumersController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Endpoint para la consulta de consumidores
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get consumidores
        ///     ## Url
        ///     GET /Consumers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de consumidores.</returns>
        [Authorize(Policy = AuthorizationPolicies.AdminPolicy )]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ConsumerResponse>>> GetConsumers()
        {
            _logger.LogInformation("Entrando al método que consulta los consumidores");
            try
            {
                var query = new ConsumersQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los consumidores. Exception: " + ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Endpoint para la consulta de consumidores
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get consumidores
        ///     ## Url
        ///     GET /consumers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de prestadores.</returns>
        [Authorize(Policy = AuthorizationPolicies.AdminOrConsumerPolicy)]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConsumerResponse>> GetConsumerById(Guid id)
        {
            _logger.LogInformation("Entrando al método que consulta los prestadores");
            try
            {
                var query = new GetConsumerByIdQuery(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los prestadores. Exception: " + ex);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint que registra un consumidor.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post consumidor.
        ///     ## Url
        ///     POST /consumers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del nuevo registro.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> PostConsumer(ConsumerRequest valor)
        {
            _logger.LogInformation("Entrando al método que registra los consumidores");
            try
            {
                var query = new CreateConsumerCommand(valor);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un consumidor. Exception: " + ex);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);            
            }
        }
        
        [Authorize(Policy = "AdminOrConsumerPolicy" )]
        [HttpPut("{id:guid}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UpdatePasswordResponse>> UpdatePassword(Guid id, [FromBody]UpdatePasswordRequest consumer)
        {
            _logger.LogInformation("Entrando al método que registra los prestadores");
            try
            {
                var query = new UpdatePasswordCommand(consumer,id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un prestador. Exception: " + ex);
                return BadRequest(ex.Message);
            }
        }
        
        
        /// <summary>
        ///     Endpoint que actualiza un consumidor.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Actualizar consumidor.
        ///     ## Url
        ///     PUT /consumers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el objeto actualizado</returns>
        [Authorize(Policy = "AdminOrConsumerPolicy" )]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConsumerResponse>> UpdateConsumer(Guid id, [FromBody]ConsumerRequest consumer)
        {
            _logger.LogInformation("Entrando al método que modifica los consumidores");
            try
            {
                var query = new UpdateConsumerCommand(consumer,id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar modificar un consumidor. Exception: " + ex);
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint que elimina un consumer.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Eliminar consumer.
        ///     ## Url
        ///     DELETE /consumers
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del objeto eliminado</returns>
        [Authorize(Policy = "AdminPolicy" )]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteConsumer(Guid id)
        {
            _logger.LogInformation("Entrando al método que elimina un prestador");
            try
            {
                var query = new DeleteConsumerCommand(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un prestador. Exception: " + ex);
                return BadRequest(ex.Message);
            }
        }
        
}