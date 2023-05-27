using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Base;

namespace UCABPagaloTodoMS.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicesController : BaseController<ServicesController>
    {
        private readonly IMediator _mediator;

        public ServicesController(ILogger<ServicesController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Endpoint para la consulta de servicios
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get servicios
        ///     ## Url
        ///     GET /services
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de servicios.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ServiceResponse>>> GetServices()
        {
            _logger.LogInformation("Entrando al método que consulta los servicios");
            try
            {
                var query = new GetServicesQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los servicios. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint para la consulta de servicios por id
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get servicios
        ///     ## Url
        ///     GET /services/{id}
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el servicio con el id que se paso.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ServiceResponse>>> GetServiceById(Guid id)
        {
            _logger.LogInformation("Entrando al método que consulta los servicios dado el id");
            try
            {
                var query = new GetServiceByIdQuery(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los servicios dado el id. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint para la consulta de servicios por prestador
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Get servicios por prestador
        ///     ## Url
        ///     GET /services/provider/{id}
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna los servicios del prestador.</returns>
        [HttpGet("Provider/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ServiceResponse>>> GetServiceByProviderId(Guid id)
        {
            _logger.LogInformation("Entrando al método que consulta los servicios dado el id de un prestador");
            try
            {
                var query = new GetServicesByProviderIdQuery(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los servicios dado el id. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint que registra un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post registra servicio.
        ///     ## Url
        ///     POST /services
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del nuevo registro.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateService(ServiceRequest request)
        {
            _logger.LogInformation("Entrando al método que registra los servicios");
            try
            {
                AddServiceValidator validator = new AddServiceValidator();
                validator.ValidateAndThrow(request);
                var query = new CreateServiceCommand(request);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un servicio. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
               
        /// <summary>
        ///     Endpoint que actualiza un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Patch actualiza un servicio.
        ///     ## Url
        ///     PATCH /services/{id}
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el regisrto modificado.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ServiceResponse>> UpdateService([FromBody] ServiceRequest request, Guid id)
        {
            _logger.LogInformation("Entrando al método que actualiza un servicio");
            try
            {   
                AddServiceValidator validator = new AddServiceValidator();
                validator.ValidateAndThrow(request);
                var query = new UpdateServiceCommand(request, id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar actualizar un servicio. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint que elimina un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Delete elimina un servicio.
        ///     ## Url
        ///     DELETE /services/{id}
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del registro modificado.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteService(Guid id)
        {
            _logger.LogInformation("Entrando al método que elimina un servicio");
            try
            {
                var query = new DeleteServiceCommand(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar eliminar un servicio. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
        
        /// <summary>
        ///     Endpoint que actualiza los campos de un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Patch actualiza los campos de un servicio.
        ///     ## Url
        ///     PATCH /services/{id}/fields
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de campos modificados.</returns>
        [HttpPatch("fields/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<FieldResponse>>> UpdateField([FromBody] FieldRequest request, Guid id)
        {
            _logger.LogInformation("Entrando al método que actualiza los campos de un servicio");
            try
            {   
                FieldValidator validator = new FieldValidator();
                validator.ValidateAndThrow(request); 
                var query = new UpdateFieldCommand(request, id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar actualizar los campos de un servicio. Exception: " + ex.Message);
                return BadRequest(ex.Message+ "\n" +ex.InnerException?.Message);
            }
        }


        /// <summary>
        ///     Endpoint que registra un formato de archivo de conciliacion
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post registra campos de archivo de conciliacion.
        ///     ## Url
        ///     POST /format
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna la lista de los ids de los campo .</returns>
        [HttpPost("format")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Guid>>> CreateFormat(List<FieldRequest> fieldsRequests)
        {
            _logger.LogInformation("Entrando al método que registra el formato de conciliacion dado un servicio");
            try
            {
                FieldValidator validator = new FieldValidator();
                foreach (var fieldRequest in fieldsRequests)
                {
                    validator.ValidateAndThrow(fieldRequest);
                }
                List<Guid> responsesList = new();
                foreach (var fieldRequest in fieldsRequests)
                {
                    var query = new CreateFieldCommand(fieldRequest);
                    responsesList.Add((await _mediator.Send(query)));
                }
                return Ok(responsesList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un campo. Exception: " + ex.Message);
                return BadRequest(ex.Message+"\n"+ex.InnerException?.Message);
            }
        }
    }
    