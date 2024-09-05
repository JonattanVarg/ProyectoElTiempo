using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos.JobApplication.Base
{
    /// <summary>
    /// Clase base para el Dto  de creación de aplicaciones a ofertas de trabajo.
    /// </summary>
    public abstract class JobApplicationBaseDto
    {
        /// <summary>
        /// Nombre del candidato que realiza la aplicación.
        /// </summary>
        [Required(ErrorMessage = "El nombre del candidato es obligatorio.")]
        [SwaggerSchema("El nombre completo del candidato.")]
        public string CandidateName { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del candidato que realiza la aplicación.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico del candidato es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico debe ser una dirección de correo válida.")]
        [SwaggerSchema("La dirección de correo electrónico del candidato.")]
        public string CandidateEmail { get; set; } = string.Empty;

        /// <summary>
        /// ID de la oferta de trabajo a la que se aplica.
        /// </summary>
        [Required(ErrorMessage = "El ID de la oferta de trabajo es obligatorio.")]
        [SwaggerSchema("El identificador único de la oferta de trabajo.")]
        public int JobOfferId { get; set; }
    }
}
