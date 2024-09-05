    using API.Dtos.JobApplication;
    using API.Dtos.JobOffer;
    using API.Dtos.Responses;
    using API.Services.Interfaces;
    using Azure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;

    namespace API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class JobApplicationsController : ControllerBase
        {
            private readonly IJobApplicationService _jobApplicationService;
            private readonly ILogger<JobApplicationsController> _logger;

            public JobApplicationsController(IJobApplicationService jobApplicationService, ILogger<JobApplicationsController> logger)
            {
                _jobApplicationService = jobApplicationService;
                _logger = logger;
            }

            /// <summary>
            /// Obtiene todas las aplicaciones de trabajo.
            /// </summary>
            /// <returns>Una lista de todas las aplicaciones de trabajo.</returns>
            /// <response code="200">Operación exitosa. Devuelve la lista de aplicaciones de trabajo.</response>
            /// <response code="500">Error interno del servidor.</response>
            [HttpGet]
            [Authorize(Roles = "Admin,Reclutador")]
            [SwaggerOperation(Summary = "Obtiene todas las aplicaciones de trabajo",
                              Description = "Recupera una lista de todas las aplicaciones de trabajo disponibles.")]
            [SwaggerResponse(200, "Operación exitosa", typeof(GenericResponseDto<IEnumerable<JobApplicationDto>>))]
            [SwaggerResponse(500, "Error interno del servidor")]
            public async Task<IActionResult> GetAllJobApplications()
            {
                try
                {
                    _logger.LogInformation("Recuperando todas las aplicaciones de trabajo.");
                    var response = await _jobApplicationService.GetAllJobApplicationsAsync();

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al recuperar todas las aplicaciones de trabajo.");
                    var response = new GenericResponseDto<IEnumerable<JobApplicationDto>>
                    {
                        IsSuccess = false,
                        Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.",
                        Data = null
                    };
                    return StatusCode(500, response);
                }
            }

            /// <summary>
            /// Obtiene una aplicación de trabajo por su ID.
            /// </summary>
            /// <param name="id">El ID de la aplicación de trabajo que se desea recuperar.</param>
            /// <returns>Una aplicación de trabajo si se encuentra; de lo contrario, un error 404.</returns>
            /// <response code="200">Operación exitosa. Devuelve la aplicación de trabajo.</response>
            /// <response code="404">No se encontró la aplicación de trabajo con el ID proporcionado.</response>
            /// <response code="500">Error interno del servidor.</response>
            [HttpGet("{id}")]
            [Authorize(Roles = "Admin,Reclutador")]
            [SwaggerOperation(Summary = "Obtiene una aplicación de trabajo por su ID",
                              Description = "Recupera una aplicación de trabajo específica utilizando su ID.")]
            [SwaggerResponse(200, "Operación exitosa", typeof(GenericResponseDto<JobApplicationDto>))]
            [SwaggerResponse(404, "No se encontró la aplicación de trabajo con el ID proporcionado")]
            [SwaggerResponse(500, "Error interno del servidor")]
            public async Task<IActionResult> GetJobApplicationById(int id)
            {
                try
                {
                    _logger.LogInformation("Recuperando la aplicación de trabajo con ID: {Id}.", id);
                    var response = await _jobApplicationService.GetJobApplicationByIdAsync(id);

                    if (!response.IsSuccess && response.Data == null) return NotFound(response);

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al recuperar la aplicación de trabajo con ID: {Id}.", id);
                    var response = new GenericResponseDto<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                    };
                    return StatusCode(500, response);
                }
            }

            /// <summary>
            /// Crea una nueva aplicación de trabajo. (NOTA: Se verifica internamente que JobOfferId corresponda a un ID de oferta de trabajo existente)
            /// </summary>
            /// <param name="jobApplicationCreateDto">Los datos de la aplicación de trabajo a crear.</param>
            /// <returns>Un resultado que indica la creación exitosa.</returns>
            /// <response code="201">Operación exitosa. Devuelve la aplicación de trabajo creada.</response>
            /// <response code="400">Solicitud inválida. El modelo de datos no es válido o la oferta de trabajo no existe.</response>
            /// <response code="500">Error interno del servidor.</response>
            [HttpPost]
            [Authorize(Roles = "Admin,Reclutador,Candidato")]
            [SwaggerOperation(Summary = "Crea una nueva aplicación de trabajo",
                              Description = "Permite crear una nueva aplicación de trabajo proporcionando los datos necesarios.")]
            [SwaggerResponse(201, "Operación exitosa", typeof(GenericResponseDto<JobApplicationDto>))]
            [SwaggerResponse(400, "Solicitud inválida")]
            [SwaggerResponse(500, "Error interno del servidor")]
            public async Task<IActionResult> CreateJobApplication([FromBody] JobApplicationCreateDto jobApplicationCreateDto)
            {
                var response = new GenericResponseDto<JobApplicationDto>();

                try
                {
                    if (!ModelState.IsValid)
                    {
                        _logger.LogWarning("Modelo de datos no válido para la creación de la aplicación de trabajo.");
                        response = new GenericResponseDto<JobApplicationDto>
                        {
                            IsSuccess = false,
                            Message = "Modelo de datos no válido.",
                            Data = null
                        };
                        return BadRequest(response);
                    }

                    _logger.LogInformation("Iniciando solicitud para agregar aplicación de trabajo");
                    response = await _jobApplicationService.AddJobApplicationAsync(jobApplicationCreateDto);

                    if (!response.IsSuccess && response.Data == null) return NotFound(response);

                    return CreatedAtAction(nameof(GetJobApplicationById), new { id = response.Data!.Id }, response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al crear una nueva aplicación de trabajo.");
                    response = new GenericResponseDto<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                    };
                    return StatusCode(500, response);
                }
            }

            /// <summary>
            /// Elimina una aplicación de trabajo por su ID.
            /// </summary>
            /// <param name="id">El ID de la aplicación de trabajo que se va a eliminar.</param>
            /// <returns>Un objeto JobResponseDto<int> que contiene el ID de la aplicación de trabajo eliminada.</returns>
            /// <response code="200">Operación exitosa. Devuelve el ID de la aplicación de trabajo eliminada.</response>
            /// <response code="404">No se encontró la aplicación de trabajo con el ID proporcionado.</response>
            /// <response code="500">Error interno del servidor.</response>
            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin,Reclutador")]
            [SwaggerOperation(Summary = "Elimina una aplicación de trabajo existente",
                              Description = "Permite eliminar una aplicación de trabajo existente utilizando su ID.")]
            [SwaggerResponse(200, "Operación exitosa. Devuelve el ID de la aplicación de trabajo eliminada.", typeof(GenericResponseDto<int>))]
            [SwaggerResponse(404, "No se encontró la aplicación de trabajo con el ID proporcionado.")]
            [SwaggerResponse(500, "Error interno del servidor")]
            public async Task<IActionResult> DeleteJobApplication(int id)
            {
                try
                {
                    _logger.LogInformation("Iniciando la solicitud para aliminar la aplicación de trabajo con ID: {Id}.", id);
                    var response = await _jobApplicationService.DeleteJobApplicationAsync(id);

                    if (!response.IsSuccess && response.Data == null) return NotFound(response);

                    _logger.LogInformation("Aplciación de trabajo con ID: {Id} eliminada exitosamente.", id);

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al eliminar la aplicación de trabajo con ID: {Id}.", id);
                    var response = new GenericResponseDto<int>
                    {
                        IsSuccess = false,
                        Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                    };
                    return StatusCode(500, response);
                }
            }
        }
    }
