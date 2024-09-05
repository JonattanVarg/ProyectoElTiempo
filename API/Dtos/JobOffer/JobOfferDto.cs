using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using API.Dtos.JobApplication;

namespace API.Dtos.JobOffer
{
    /// <summary>
    /// Dto principal para las ofertas de trabajo, mayormente  usado para los Gets y Responses -para informar oferta de trabajo creada-
    /// del controller de  ofertas de trabajo (sin JobApplications)
    /// </summary>
    public class JobOfferDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    }
}
