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
                return BadRequest(ex);
            }
        }

        /// <summary>
        ///     Endpoint que registra un consumidor.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post proveedor.
        ///     ## Url
        ///     POST /Consumers
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
                _logger.LogError("Ocurrio un error al intentar registrar un proveedor. Exception: " + ex);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);            }
        }

}