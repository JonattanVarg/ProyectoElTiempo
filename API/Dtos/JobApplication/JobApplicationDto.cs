using System.ComponentModel.DataAnnotations;

namespace API.Dtos.JobApplication
{
    public class JobApplicationDto
    {
        /// <summary>
        /// Dto principal para las aplicaciones a ofertas de trabajo, mayormente usado para los Gets y Responses -para informar aplicación a oferta de trabajo creada-
        /// del controller de aplicacioens a ofertas de trabajo (sin JobOffer)
        /// </summary>
        public int Id { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public int JobOfferId { get; set; }
        public DateTime DateApplied { get; set; } = DateTime.UtcNow;
    }
}
