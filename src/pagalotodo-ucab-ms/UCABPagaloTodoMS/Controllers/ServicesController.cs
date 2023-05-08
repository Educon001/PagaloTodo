using FluentValidation;
using GreenPipes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Application.Validators;
using UCABPagaloTodoMS.Base;
using ValidationResult = FluentValidation.Results.ValidationResult;

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
                return BadRequest(ex);
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
                var query = new GetServicesByIdQuery(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error en la consulta de los servicios. Exception: " + ex.Message);
                return BadRequest(ex);
            }
        }
        
        /// <summary>
        ///     Endpoint que registra un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Post registra servicio.
        ///     ## Url
        ///     POST /servicio
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
                ValidationResult result = validator.Validate(request);
                if (!result.IsValid)
                {
                    validator.ValidateAndThrow(request);
                }
                var query = new CreateServiceCommand(request);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError($"{error.ErrorMessage}\n");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar registrar un servicio. Exception: " + ex.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Endpoint que actualiza un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Patch actualiza un servicio.
        ///     ## Url
        ///     PATCH /servicio
        /// </remarks>
        /// <response code="200">
        ///     Accepted:
        ///     - Operation successful.
        /// </response>
        /// <returns>Retorna el id del registro modificado.</returns>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ServiceResponse>> UpdateService([FromBody] ServiceRequest request, Guid id)
        {
            _logger.LogInformation("Entrando al método que actualiza un servicio");
            try
            {   
                AddServiceValidator validator = new AddServiceValidator();
                ValidationResult result = validator.Validate(request);
                if (!result.IsValid)
                {
                    validator.ValidateAndThrow(request);
                }
                var query = new UpdateServiceCommand(request, id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError($"{error.ErrorMessage}\n");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar actualizar un servicio. Exception: " + ex.Message);
                throw;
            }
        }
        
        /// <summary>
        ///     Endpoint que actualiza un servicio.
        /// </summary>
        /// <remarks>
        ///     ## Description
        ///     ### Patch actualiza un servicio.
        ///     ## Url
        ///     PATCH /servicio
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
            _logger.LogInformation("Entrando al método que actualiza un servicio");
            try
            {
                var query = new DeleteServiceCommand(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error al intentar actualizar un servicio. Exception: " + ex.Message);
                throw;
            }
        }
    }