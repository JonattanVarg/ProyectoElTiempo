using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobApplicationRepository> _logger;

        public JobApplicationRepository(AppDbContext context, ILogger<JobApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Agrega una nueva aplicación de trabajo a la base de datos.
        /// </summary>
        /// <param name="application">El objeto <see cref="JobApplication"/> que se va a agregar.</param>
        public async Task AddJobApplicationAsync(JobApplication application)
        {
            try
            {
                _logger.LogInformation("Agregando una nueva aplicación de trabajo.");
                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Aplicación de trabajo agregada exitosamente con ID: {Id}.", application.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al agregar una nueva aplicación de trabajo.");
                throw;
            }
        }

        /// <summary>
        /// Elimina una aplicación de trabajo existente de la base de datos.
        /// </summary>
        /// <param name="id">El ID de la aplicación de trabajo que se va a eliminar.</param>
        /// <returns>.</returns>
        public async Task DeleteJobApplicationAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando la aplicación de trabajo con ID: {Id} para eliminarla.", id);
                var application = await _context.JobApplications.FindAsync(id);
                if (application != null)
                {
                    _context.JobApplications.Remove(application);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Aplicación de trabajo con ID: {Id} eliminada exitosamente.", id);
                }
                else
                {
                    _logger.LogWarning("No se encontró la aplicación de trabajo con ID: {Id} para eliminar.", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al eliminar la aplicación de trabajo con ID: {Id}.", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las aplicaciones de trabajo de la base de datos.
        /// </summary>
        /// <returns>Una lista de todas las aplicaciones de trabajo.</returns>
        public async Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync()
        {
            try
            {
                _logger.LogInformation("Recuperando todas las aplicaciones de trabajo desde la base de datos.");
                return await _context.JobApplications.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar todas las aplicaciones de trabajo.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una aplicación de trabajo por su ID.
        /// </summary>
        /// <param name="id">El ID de la aplicación de trabajo que se desea recuperar.</param>
        /// <returns>La aplicación de trabajo si se encuentra, de lo contrario, null.</returns>
        public async Task<JobApplication> GetJobApplicationByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Recuperando la aplicación de trabajo con ID: {Id} desde la base de datos.", id);

                var jobApplication = await _context.JobApplications.FindAsync(id);

                if (jobApplication == null)
                {
                    _logger.LogWarning("No se encontró la aplicación de trabajo con ID: {Id}.", id);
                }

                return jobApplication;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al recuperar la aplicación de trabajo con ID: {Id}.", id);
                throw;
            }
        }
    }
}
