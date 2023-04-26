using MediatR;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Base;

namespace UCABPagaloTodoMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EjemploController : BaseController<EjemploController>
    {
        private readonly IMediator _mediator;

        public EjemploController(ILogger<EjemploController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Endpoint para la consulta de prueba
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get valores de prueba
        ///     ## Url
        ///     GET /ejemplo/valores
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de valores ejemplo.</returns>
        [HttpGet("valores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ValoresResponse>>> ConsultaValores()
        {
            _logger.LogInformation("Entrando al método que consulta los valores de ejemplo");
            try
            {
                var query = new ConsultarValoresPruebaQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los valores de prueba. Exception: " + ex);
                throw;
            }
        }

        /// <summary>
        ///     Endpoint que registra un valor.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post registra valor de prueba.
        ///     ## Url
        ///     POST /ejemplo/valor
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del nuevo registro.</returns>
        [HttpPost("valor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AgregarValor(ValoresRequest valor)
        {
            _logger.LogInformation("Entrando al método que registra los valores de prueba");
            try
            {
                var query = new AgregarValorPruebaCommand(valor);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un valor de prueba. Exception: " + ex);
                throw;
            }
        }
    }
}
