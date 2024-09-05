using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Repositories.Interfaces;
using API.Dtos.JobOffer;

namespace API.Repositories
{
    public class JobOfferRepository : IJobOfferRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobOfferRepository> _logger;

        public JobOfferRepository(AppDbContext context, ILogger<JobOfferRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las ofertas de trabajo.
        /// </summary>
        /// <returns>Una lista de todas las ofertas de trabajo.</returns>
        public async Task<IEnumerable<JobOffer>> GetAllJobOffersAsync()
        {
            try
            {
                _logger.LogInformation("Recuperando todas las ofertas de trabajo desde la base de datos.");
                return await _context.JobOffers.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar todas las ofertas de trabajo.");
                throw; 
            }
        }

        /// <summary>
        /// Obtiene una oferta de trabajo por su ID.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo.</param>
        /// <returns>Una oferta de trabajo.</returns>
        public async Task<JobOffer> GetJobOfferByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando la oferta de trabajo con ID: {Id} desde la base de datos.", id);
                var jobOffer = await _context.JobOffers
                                     .FirstOrDefaultAsync(j => j.Id == id);

                return jobOffer!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar la oferta de trabajo con ID: {Id}.", id);
                throw; 
            }
        }

        /// <summary>
        /// Obtiene una oferta de trabajo por su ID junto a sus aplicaciones de trabajo respectivas
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo.</param>
        /// <returns>Una oferta de trabajo junto a sus aplicaciones de trabajo respectivas</returns>
        public async Task<JobOffer> GetJobOfferWithApplicationsByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando la oferta de trabajo con ID: {Id} desde la base de datos junto a sus aplicaciones de trabajo respectivas.", id);
                var jobOffer = await _context.JobOffers
                                      .Include(j => j.JobApplications)
                                     .FirstOrDefaultAsync(j => j.Id == id);

                return jobOffer!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar la oferta de trabajo con ID: {Id}.", id);
                throw;
            }
        }

        /// <summary>
        /// Agrega una nueva oferta de trabajo a la base de datos.
        /// </summary>
        /// <param name="jobOffer">El objeto JobOffer que se va a agregar.</param>
        public async Task AddJobOfferAsync(JobOffer jobOffer)
        {
            try
            {
                _logger.LogInformation("Agregando una nueva oferta de trabajo.");
                _context.JobOffers.Add(jobOffer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Nueva oferta de trabajo agregada exitosamente con ID: {Id}.", jobOffer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al agregar una nueva oferta de trabajo.");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una oferta de trabajo existente en la base de datos.
        /// </summary>
        /// <param name="jobOffer">El objeto JobOffer con los datos actualizados.</param>
        public async Task UpdateJobOfferAsync(JobOffer jobOffer)
        {
            try
            {
                var existingJobOffer = await _context.JobOffers
                                                 .FirstOrDefaultAsync(j => j.Id == jobOffer.Id);

                if (existingJobOffer == null) throw new Exception($"No se encontró la oferta de trabajo con ID: {jobOffer.Id}.");

                existingJobOffer!.Title = jobOffer.Title;
                existingJobOffer!.Description = jobOffer.Description;
                existingJobOffer!.Location = jobOffer.Location;
                existingJobOffer!.Salary = jobOffer.Salary;
                existingJobOffer!.ContractType = jobOffer.ContractType;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Oferta de trabajo con ID: {Id} actualizada exitosamente.", jobOffer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al actualizar la oferta de trabajo con ID: {Id}.", jobOffer.Id);
                throw; 
            }
        }

        /// <summary>
        /// Elimina una oferta de trabajo existente en la base de datos.
        /// </summary>
        /// <param name="id">El ID de la oferta de trabajo que se va a eliminar.</param>
        public async Task DeleteJobOfferAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando la oferta de trabajo con ID: {Id} para eliminarla.", id);
                var jobOffer = await _context.JobOffers.FindAsync(id);
                if (jobOffer != null)
                {
                    _context.JobOffers.Remove(jobOffer);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Oferta de trabajo con ID: {Id} eliminada exitosamente.", id);
                }
                else
                {
                    _logger.LogWarning("No se encontró la oferta de trabajo con ID: {Id} para eliminar.", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al eliminar la oferta de trabajo con ID: {Id}.", id);
                throw;
            }
        }
    }
}
