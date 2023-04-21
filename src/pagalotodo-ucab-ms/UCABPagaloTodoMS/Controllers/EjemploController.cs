using MediatR;
using Microsoft.AspNetCore.Mvc;
using NASSA.Utils.BaseController;
using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Queries;

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
        [ProducesResponseType(typeof(Response200<string>), 200)]
        [ProducesResponseType(typeof(Response400), 400)]
        public async Task<ActionResult<List<string>>> GetBrokers()
        {
            _logger.LogInformation("Entrando al método que consulta los valores de ejemplo");
            try
            {
                var query = new ConsultarValoresPruebaQuery();
                var response = await _mediator.Send(query);
                return Response200(NewResponseOperation(), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los valores de prueba. Exception: " + ex);
                return Response400(ex.Message, "Ocurrio un error en la consulta de los valores de prueba.");
            }
        }

        /// <summary>
        ///     Endpoint que registra un valor de ejemplo
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post reistra valor de prueba
        ///     ## Url
        ///     GET /ejemplo/valor
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Returns the list of banks.</returns>
        [HttpPost("valor")]
        [ProducesResponseType(typeof(Response200<List<string>>), 200)]
        [ProducesResponseType(typeof(Response400), 400)]
        public async Task<ActionResult<List<string>>> GetBanks(string valor)
        {
            _logger.LogInformation("Entrando al método que registra los valores de prueba");
            try
            {
                var query = new AgregarValorPruebaCommand(valor);
                var response = await _mediator.Send(query);
                return Response200(NewResponseOperation(), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un valor de prueba. Exception: " + ex);
                return Response400(ex.Message, "\"Ocurrio un error al intentar registrar un valor de prueba.");
            }
        }
    }
}
