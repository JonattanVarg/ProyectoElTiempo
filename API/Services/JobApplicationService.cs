using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Dtos.Responses;
using API.Models;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IJobOfferRepository _jobOfferRepository;
        private readonly ILogger<JobApplicationService> _logger;
        private readonly IMapper _mapper;

        public JobApplicationService(IJobApplicationRepository repository, IJobOfferRepository jobOfferRepository, ILogger<JobApplicationService> logger, IMapper mapper)
        {
            _repository = repository;
            _jobOfferRepository = jobOfferRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Recupera todas las aplicaciones de trabajo.
        /// </summary>
        /// <returns>Una lista de todas las aplicaciones de trabajo.</returns>
        public async Task<GenericResponseDto<IEnumerable<JobApplicationDto>>> GetAllJobApplicationsAsync()
        {
            try
            {
                _logger.LogInformation("Recuperando todas las aplicaciones de trabajo.");
                var jobApplications = await _repository.GetAllJobApplicationsAsync();

                if (jobApplications == null || !jobApplications.Any())
                {
                    _logger.LogWarning("No se encontraron aplicaicones de trabajo disponibles.");
                    return new GenericResponseDto<IEnumerable<JobApplicationDto>>
                    {
                        IsSuccess = true,
                        Message = "No hay aplicaicones de trabajo disponibles en este momento.",
                        Data = Enumerable.Empty<JobApplicationDto>()
                    };
                }

                _logger.LogInformation("Aplicaciones de trabajo recuperadas exitosamente.");
                var mapped = _mapper.Map<IEnumerable<JobApplicationDto>>(jobApplications);

                return new GenericResponseDto<IEnumerable<JobApplicationDto>>
                {
                    IsSuccess = true,
                    Message = "Ofertas de trabajo recuperadas exitosamente.",
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar todas las aplicaciones de trabajo.");
                throw;
            }
        }

        /// <summary>
        /// Recupera una aplicación de trabajo por su ID.
        /// </summary>
        /// <param name="id">El ID de la aplicación de trabajo que se desea recuperar.</param>
        /// <returns>Un objeto <see cref="JobApplicationDto"/> que representa la aplicación de trabajo</returns>
        public async Task<GenericResponseDto<JobApplicationDto>> GetJobApplicationByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando la aplicación de trabajo con ID: {Id}.", id);
                var jobApplication = await _repository.GetJobApplicationByIdAsync(id);

                if (jobApplication == null)
                {
                    _logger.LogWarning("No se encontró la aplicación de trabajo con ID: {Id}", id);
                    return new GenericResponseDto<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la aplicación de trabajo.",
                        Data = null
                    };
                }

                var mapped = _mapper.Map<JobApplicationDto>(jobApplication);
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = true,
                    Message = "Aplicación de trabajo recuperada exitosamente.",
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar la aplicación de trabajo con ID: {Id}.", id);
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                throw;
            }
        }

        /// <summary>
        /// Agrega una nueva aplicación de trabajo al repositorio.
        /// </summary>
        /// <param name="jobApplicationCreateDto">El objeto <see cref="JobApplicationCreateDto"/> que contiene los datos de la aplicación de trabajo a agregar.</param>
        /// <returns>El objeto <see cref="JobApplicationDto"/> que representa la aplicación de trabajo creada.</returns>
        /// <exception cref="ArgumentException">Lanzada si la oferta de trabajo asociada no existe.</exception>
        /// <exception cref="Exception">Lanzada si ocurre un error durante la operación.</exception>
        public async Task<GenericResponseDto<JobApplicationDto>> AddJobApplicationAsync(JobApplicationCreateDto jobApplicationCreateDto)
        {
            try
            {
                // Se verifica primero si la oferta de trabajo a la cual se está aplicando existe
                _logger.LogInformation("Verificando si la oferta de trabajo con ID: {JobOfferId} existe.", jobApplicationCreateDto.JobOfferId);
                var jobOffer = await _jobOfferRepository.GetJobOfferByIdAsync(jobApplicationCreateDto.JobOfferId);
                if (jobOffer == null)
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {JobOfferId}.", jobApplicationCreateDto.JobOfferId);
                    return new GenericResponseDto<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la oferta de trabajo para registra las aplicación a dicha oferta.",
                        Data = null
                    };
                }

                var jobApplication = _mapper.Map<JobApplication>(jobApplicationCreateDto);
                jobApplication.JobOffer = jobOffer;

                _logger.LogInformation("Agregando una nueva aplicación de trabajo.");
                await _repository.AddJobApplicationAsync(jobApplication);

                var jobApplicationDto = _mapper.Map<JobApplicationDto>(jobApplication);
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = true,
                    Message = "Se aplicó a la oferta de trabajo exitosamente.",
                    Data = jobApplicationDto
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error en los datos de entrada al intentar agregar una nueva aplicación de trabajo.");
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = false,
                    Message = "Error en los datos de entrada al intentar agregar una nueva aplicación de trabajo."
                };
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al agregar una nueva aplicación de trabajo.");
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error al agregar una nueva aplicación de trabajo"
                };
                throw;
            }
        }

        /// <summary>
        /// Elimina una aplicación de trabajo existente del repositorio.
        /// </summary>
        /// <param name="id">El ID de la aplicación de trabajo que se va a eliminar.</param>
        public async Task<GenericResponseDto<JobApplicationDto>> DeleteJobApplicationAsync(int id)
        {
            try
            {
                // Verificar si la aplicación de trabajo con el ID especificado existe
                _logger.LogInformation("Recuperando la aplicación de trabajo con ID: {Id} desde el repositorio.", id);
                var jobApplication = await _repository.GetJobApplicationByIdAsync(id);

                if (jobApplication == null)
                {
                    _logger.LogWarning("No se encontró la aplicación de trabajo con ID: {Id}", id);
                    return new GenericResponseDto<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la aplicación de trabajo.",
                        Data = null
                    };
                }

                _logger.LogInformation("Eliminando la aplicación de trabajo con ID: {Id}.", id);
                await _repository.DeleteJobApplicationAsync(id);

                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = true,
                    Message = "Aplicación de trabajo elimianada exitosamente.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al eliminar la aplicación de trabajo con ID: {Id}.", id);
                return new GenericResponseDto<JobApplicationDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno al eliminar la aplicación de trabajo.",
                    Data = null
                };
                throw;
            }
        }
    }
}
