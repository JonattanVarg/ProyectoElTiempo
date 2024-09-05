using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Dtos.Responses;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class JobOfferService : IJobOfferService
    {
        private readonly IJobOfferRepository _repository;
        private readonly ILogger<JobOfferService> _logger; 
        private readonly IMapper _mapper;


        public JobOfferService(IJobOfferRepository repository, ILogger<JobOfferService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Recupera todas las ofertas de trabajo del repositorio.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="GenericResponseDto<IEnumerable<JobOfferDto>>>"/> que representan todas las ofertas de trabajo.</returns>
        public async Task<GenericResponseDto<IEnumerable<JobOfferDto>>> GetAllJobOffersAsync()
        {
            try
            {
                _logger.LogInformation("Recuperando todas las ofertas de trabajo desde el repositorio.");

                var jobOffers = await _repository.GetAllJobOffersAsync();

                if (jobOffers == null || !jobOffers.Any())
                {
                    _logger.LogWarning("No se encontraron ofertas de trabajo disponibles.");
                    return new GenericResponseDto<IEnumerable<JobOfferDto>>
                    {
                        IsSuccess = true,
                        Message = "No hay ofertas de trabajo disponibles en este momento.",
                        Data = Enumerable.Empty<JobOfferDto>()
                    };
                }

                _logger.LogInformation("Ofertas de trabajo recuperadas exitosamente.");
                var mappedJobOffers = _mapper.Map<IEnumerable<JobOfferDto>>(jobOffers);

                return new GenericResponseDto<IEnumerable<JobOfferDto>>
                {
                    IsSuccess = true,
                    Message = "Ofertas de trabajo recuperadas exitosamente.",
                    Data = mappedJobOffers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar todas las ofertas de trabajo.");
                return new GenericResponseDto<IEnumerable<JobOfferDto>>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno al recuperar las ofertas de trabajo.",
                    Data =null
                };
                throw;
            }
        }

        /// <summary>
        /// Recupera una oferta de trabajo por su ID desde el repositorio.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo que se desea recuperar.</param>
        /// <returns>Un objeto <see cref="GenericResponseDto{JobOfferDto}"/> que representa la oferta de trabajo, o un error 404 si no se encuentra.</returns>
        public async Task<GenericResponseDto<JobOfferDto>> GetJobOfferByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando la oferta de trabajo con ID: {Id} desde el repositorio.", id);
                var jobOffer = await _repository.GetJobOfferByIdAsync(id);

                if (jobOffer == null)
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {Id}", id);
                    return new GenericResponseDto<JobOfferDto>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la oferta de trabajo.",
                        Data = null
                    };
                }

                var jobOfferDto = _mapper.Map<JobOfferDto>(jobOffer);
                return new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = true,
                    Message = "Oferta de trabajo recuperada exitosamente.",
                    Data = jobOfferDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar la oferta de trabajo con ID: {Id}.", id);
                return new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                };
                throw;
            }
        }


        /// <summary>
        /// Agrega una nueva oferta de trabajo al repositorio.
        /// </summary>
        /// <param name="jobOfferCreateDto">El objeto <see cref="JobOfferCreateDto"/> que contiene los datos de la oferta de trabajo a agregar.</param>
        /// <returns>Un objeto <see cref="GenericResponseDto{JobOfferDto}"/> que contiene el resultado de la operación.</returns>
        public async Task<GenericResponseDto<JobOfferDto>> AddJobOfferAsync(JobOfferCreateDto jobOfferCreateDto)
        {
            var response = new GenericResponseDto<JobOfferDto>();

            try
            {
                var jobOffer = _mapper.Map<JobOffer>(jobOfferCreateDto);
                _logger.LogInformation("Agregando una nueva oferta de trabajo con Título: {Title}", jobOffer.Title);

                await _repository.AddJobOfferAsync(jobOffer);

                response.IsSuccess = true;
                response.Message = "Oferta de trabajo creada exitosamente.";
                response.Data = _mapper.Map<JobOfferDto>(jobOffer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al agregar una nueva oferta de trabajo.");
                response.IsSuccess = false;
                response.Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.";
                response.Data = null;
                throw;
            }

            return response;
        }

        /// <summary>
        /// Actualiza una oferta de trabajo existente en el repositorio.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo a actualizar.</param>
        /// <param name="jobOfferUpdateDto">El objeto <see cref="JobOfferUpdateDto"/> que contiene los datos actualizados de la oferta de trabajo.</param>
        public async Task<GenericResponseDto<JobOfferDto>> UpdateJobOfferAsync(int id, JobOfferUpdateDto jobOfferUpdateDto)
        {
            var response = new GenericResponseDto<JobOfferDto>();

            try
            {
                // Verificar si la oferta de trabajo con el ID especificado existe
                var jobOffer = await _repository.GetJobOfferByIdAsync(id);

                if (jobOffer == null)
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {Id}", id);

                    response.IsSuccess = false;
                    response.Message = "No se encontró la oferta de trabajo";
                    response.Data = null;

                    return response;
                }

                // Asignar la actualización DTO a la entidad de oferta de trabajo existente
                jobOffer.Title = jobOfferUpdateDto.Title;
                jobOffer.Description = jobOfferUpdateDto.Description;
                jobOffer.Location = jobOfferUpdateDto.Location;
                jobOffer.Salary = jobOfferUpdateDto.Salary;
                jobOffer.ContractType = jobOfferUpdateDto.ContractType;

                await _repository.UpdateJobOfferAsync(jobOffer);
                _logger.LogInformation("Actualizando la oferta de trabajo con ID: {Id} en el repositorio.", jobOffer.Id);

                response.IsSuccess = true;
                response.Message = "Oferta de trabajo actualizada exitosamente.";
                response.Data = _mapper.Map<JobOfferDto>(jobOffer); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al actualizar la oferta de trabajo con ID: {Id}.", id);
                response.IsSuccess = false;
                response.Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.";
                response.Data = null;
                throw;
            }

            return response;
        }

        /// <summary>
        /// Elimina una oferta de trabajo existente del repositorio.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo que se va a eliminar.</param>
        /// <exception cref="Exception">Lanzada si ocurre un error durante la operación.</exception>
        public async Task<GenericResponseDto<JobOfferDto>> DeleteJobOfferAsync(int id)
        {
            try
            {
                // Verificar si la oferta de trabajo con el ID especificado existe
                _logger.LogInformation("Recuperando la oferta de trabajo con ID: {Id} desde el repositorio.", id);
                var jobOffer = await _repository.GetJobOfferByIdAsync(id);

                if (jobOffer == null)
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {Id}", id);
                    return new GenericResponseDto<JobOfferDto>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la oferta de trabajo.",
                        Data = null
                    };
                }

                _logger.LogInformation("Eliminando la oferta de trabajo con ID: {Id} del repositorio.", id);
                await _repository.DeleteJobOfferAsync(id);

                return new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = true,
                    Message = "Oferta de trabajo eliminada exitosamente.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponseDto<JobOfferDto>
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno al eliminar la oferta de trabajo.",
                    Data = null
                };
                throw;
            }
        }

        /// <summary>
        /// Recupera todas las aplicaciones de trabajo asociadas a una oferta de trabajo específica.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo cuya colección de aplicaciones se desea recuperar.</param>
        /// <returns>Una lista de objetos <see cref="JobApplicationDto"/> que representan las aplicaciones de trabajo.</returns>
        public async Task<GenericResponseDto<IEnumerable<JobApplicationDto>>> GetJobApplicationsByJobOfferIdAsync(int id)
        {
            try
            {
                // Verificar si la oferta de trabajo con el ID especificado existe
                _logger.LogInformation("Verificando si la oferta de trabajo con ID: {Id} existe.", id);
                var jobOffer = await _repository.GetJobOfferWithApplicationsByIdAsync(id);
                if (jobOffer == null)
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {Id}.", id);
                    return new GenericResponseDto<IEnumerable<JobApplicationDto>>
                    {
                        IsSuccess = false,
                        Message = "No se encontró la oferta de trabajo con el ID especificado.",
                        Data = Enumerable.Empty<JobApplicationDto>()
                    };
                }

                // Verificar si la oferta de trabajo tiene aplicaciones
                var jobApplications = jobOffer.JobApplications;
                if (jobApplications == null || !jobApplications.Any())
                {
                    _logger.LogWarning("No se encontraron aplicaciones de trabajo para la oferta de trabajo con ID: {Id}.", id);
                    return new GenericResponseDto<IEnumerable<JobApplicationDto>>
                    {
                        IsSuccess = true,
                        Message = "No se encontraron aplicaciones de trabajo para la oferta de trabajo especificada.",
                        Data = Enumerable.Empty<JobApplicationDto>()
                    };
                }

                _logger.LogInformation("Aplicaciones de trabajo recuperadas exitosamente para la oferta de trabajo con ID: {Id}.", id);
                return new GenericResponseDto<IEnumerable<JobApplicationDto>>
                {
                    IsSuccess = true,
                    Message = "Aplicaciones de trabajo recuperadas exitosamente.",
                    Data = _mapper.Map<IEnumerable<JobApplicationDto>>(jobApplications)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar las aplicaciones de trabajo para la oferta de trabajo con ID: {Id}.", id);
                throw;
            }
        }
    }
}
