using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Dtos.Responses;
using API.Models;
using API.Services.Interfaces;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOffersController : ControllerBase
    {
        private readonly IJobOfferService _jobOfferService;
        private readonly ILogger<JobOffersController> _logger;

        public JobOffersController(IJobOfferService jobOfferService, ILogger<JobOffersController> logger)
        {
            _jobOfferService = jobOfferService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las ofertas de trabajo.
        /// </summary>
        /// <returns>Una lista de ofertas de trabajo.</returns>
        /// <response code="200">Operación exitosa. Devuelve la lista de ofertas de trabajo.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Reclutador,Candidato")]
        [SwaggerOperation(Summary = "Obtiene todas las ofertas de trabajo",
                          Description = "Recupera una lista de todas las ofertas de trabajo disponibles.")]
        [SwaggerResponse(200, "Operación exitosa", typeof(GenericResponseDto<IEnumerable<JobOfferDto>>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetAllJobOffers()
        {
            try
            {
                _logger.LogInformation("Iniciando solicitud para recuperar todas las ofertas de trabajo.");

                var response = await _jobOfferService.GetAllJobOffersAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado al obtener todas las ofertas de trabajo.");
                return StatusCode(500, new GenericResponseDto<IEnumerable<JobOfferDto>>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.",
                    Data = null
                }); ;
            }
        }

        /// <summary>
        /// Obtiene una oferta de trabajo por su ID.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo.</param>
        /// <returns>Una oferta de trabajo si se encuentra; de lo contrario, un error 404.</returns>
        /// <response code="200">Operación exitosa. Devuelve la oferta de trabajo.</response>
        /// <response code="404">No se encontró la oferta de trabajo con el ID proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Reclutador")]
        [SwaggerOperation(Summary = "Obtiene una oferta de trabajo por su ID",
                          Description = "Recupera una oferta de trabajo específica utilizando su ID.")]
        [SwaggerResponse(200, "Operación exitosa", typeof(GenericResponseDto<JobOfferDto>))]
        [SwaggerResponse(404, "No se encontró la oferta de trabajo con el ID proporcionado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetJobOfferById(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando solicitud para recuperar la oferta de trabajo con ID: {Id}", id);
                var response = await _jobOfferService.GetJobOfferByIdAsync(id);

                if (!response.IsSuccess  && response.Data == null) return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado al obtener todas las ofertas de trabajo.");
                return StatusCode(500, new GenericResponseDto<IEnumerable<JobOfferDto>>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }

        /// <summary>
        /// Crea una nueva oferta de trabajo.
        /// </summary>
        /// <param name="jobOfferCreateDto">El DTO que contiene los datos de la oferta de trabajo.</param>
        /// <returns>La oferta de trabajo creada.</returns>
        /// <response code="201">Operación exitosa. Devuelve la oferta de trabajo creada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Reclutador")]
        [SwaggerOperation(Summary = "Crea una nueva oferta de trabajo",
                          Description = "Permite crear una nueva oferta de trabajo proporcionando los datos necesarios.")]
        [SwaggerResponse(201, "Operación exitosa", typeof(GenericResponseDto<JobOfferDto>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> CreateJobOffer([FromBody] JobOfferCreateDto jobOfferCreateDto)
        {
            var response = new GenericResponseDto<JobOfferDto>();

            try
            {
                if (!ModelState.IsValid)
                {
                     response = new GenericResponseDto<JobOfferDto>
                    {
                        IsSuccess = false,
                        Message = "Modelo de datos no válido.",
                        Data = null
                    };
                    return BadRequest(response);
                }

                _logger.LogInformation("Iniciando solicitud para agregar oferta de trabajo");
                response = await _jobOfferService.AddJobOfferAsync(jobOfferCreateDto);

                return CreatedAtAction(nameof(GetJobOfferById), new { id = response.Data!.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al crear una nueva oferta de trabajo.");
                response = new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Actualiza una oferta de trabajo existente.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo a actualizar.</param>
        /// <param name="jobOfferUpdateDto">El DTO que contiene los nuevos datos de la oferta de trabajo.</param>
        /// <returns>Un objeto JobResponseDto<JobOfferDto> que contiene la oferta de trabajo actualizada en caso de éxito.</returns>
        /// <response code="200">Operación exitosa. Devuelve la oferta de trabajo actualizada.</response>
        /// <response code="400">Solicitud inválida. El ID no coincide con el del modelo de datos.</response>
        /// <response code="404">No se encontró la oferta de trabajo con el ID especificado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Reclutador")]
        [SwaggerOperation(Summary = "Actualiza una oferta de trabajo existente",
                          Description = "Permite actualizar una oferta de trabajo existente utilizando su ID y los nuevos datos.")]
        [SwaggerResponse(200, "Operación exitosa. Devuelve la oferta de trabajo actualizada.", typeof(GenericResponseDto<JobOfferDto>))]
        [SwaggerResponse(404, "No se encontró la oferta de trabajo con el ID especificado.")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> UpdateJobOffer(int id, [FromBody] JobOfferUpdateDto jobOfferUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GenericResponseDto<JobOfferCreateDto>
                    {
                        IsSuccess = false,
                        Message = "Modelo de datos no válido.",
                        Data = null
                    });
                }

                _logger.LogInformation("Iniciando solicitud para actualizar la oferta de trabajo con ID: {Id}", id);
                var response = await _jobOfferService.UpdateJobOfferAsync(id, jobOfferUpdateDto);

                if (!response.IsSuccess && response.Data == null) return NotFound(response);

                _logger.LogInformation("Oferta de trabajo con ID: {Id} actualizada exitosamente.", id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al actualizar la oferta de trabajo con ID: {Id}", id);
                var response = new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Elimina una oferta de trabajo existente. (NOTA: Al eliminarse una oferta de trabajo se eliminarán las aplicaciones de trabajo asociadas a dicha oferta)
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo a eliminar.</param>
        /// <returns>Un objeto JobResponseDto<int> que contiene el ID de la oferta de trabajo eliminada.</returns>
        /// <response code="200">Operación exitosa. Devuelve el ID de la oferta de trabajo eliminada.</response>
        /// <response code="404">No se encontró la oferta de trabajo con el ID especificado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Reclutador")]
        [SwaggerOperation(Summary = "Elimina una oferta de trabajo existente",
                          Description = "Permite eliminar una oferta de trabajo existente utilizando su ID.")]
        [SwaggerResponse(200, "Operación exitosa. Devuelve el ID de la oferta de trabajo eliminada.", typeof(GenericResponseDto<int>))]
        [SwaggerResponse(404, "No se encontró la oferta de trabajo con el ID especificado.")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> DeleteJobOffer(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando solicitud para eliminar la oferta de trabajo con ID: {Id}", id);

                var response = await _jobOfferService.DeleteJobOfferAsync(id);

                if (!response.IsSuccess && response.Data == null) return NotFound(response);

                _logger.LogInformation("Oferta de trabajo con ID: {Id} eliminada exitosamente.", id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al eliminar la oferta de trabajo con ID: {Id}", id);
                var errorResponse = new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Recupera todas las aplicaciones de trabajo asociadas a una oferta de trabajo específica.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo cuya colección de aplicaciones se desea recuperar.</param>
        /// <returns>Una lista de objetos <see cref="JobApplicationDto"/> que representan las aplicaciones de trabajo.</returns>
        /// <response code="200">Operación exitosa. Devuelve la lista de aplicaciones de trabajo asociadas a la oferta.</response>
        /// <response code="404">No se encontró la oferta de trabajo con el ID proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}/applications")]
        [Authorize(Roles = "Admin,Reclutador")]
        [SwaggerOperation(Summary = "Recupera todas las aplicaciones de trabajo para una oferta de trabajo específica",
                          Description = "Recupera todas las aplicaciones de trabajo asociadas a una oferta de trabajo específica utilizando su ID.")]
        [SwaggerResponse(200, "Operación exitosa", typeof(GenericResponseDto<IEnumerable<JobApplicationDto>>))]
        [SwaggerResponse(404, "No se encontró la oferta de trabajo con el ID proporcionado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetJobApplicationsByJobOfferId(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando todas las aplicaciones de trabajo para la oferta de trabajo con ID: {Id}.", id);
                var jobApplicationsResponse = await _jobOfferService.GetJobApplicationsByJobOfferIdAsync(id);

                if (!jobApplicationsResponse.IsSuccess) return NotFound(jobApplicationsResponse);

                return Ok(jobApplicationsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar las aplicaciones de trabajo para la oferta de trabajo con ID: {Id}", id);
                var response = new GenericResponseDto<IEnumerable<JobApplicationDto>>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                return StatusCode(500, response);
            }
        }
    }
}
