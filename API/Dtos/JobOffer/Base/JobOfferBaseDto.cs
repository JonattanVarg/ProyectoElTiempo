using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace API.Dtos.JobOffer.Base
{
    /// <summary>
    /// Clase base para los Dtos de creación y actualización de ofertas de trabajo
    /// </summary>
    public abstract class JobOfferBaseDto
    {
        /// <summary>
        /// El título de la oferta de trabajo.
        /// </summary>
        [Required]
        [MinLength(5, ErrorMessage = "El título debe tener al menos 5 caracteres.")]
        [SwaggerSchema(Description = "El título de la oferta de trabajo.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// La descripción de la oferta de trabajo.
        /// </summary>
        [Required]
        [MinLength(5, ErrorMessage = "La descripción debe tener al menos 5 caracteres.")]
        [SwaggerSchema(Description = "La descripción de la oferta de trabajo.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// La ubicación de la oferta de trabajo.
        /// </summary>
        [Required]
        [MinLength(5, ErrorMessage = "La ubicación debe tener al menos 5 caracteres.")]
        [SwaggerSchema(Description = "La ubicación de la oferta de trabajo.")]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// El salario ofrecido para la oferta de trabajo.
        /// </summary>
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "El salario debe ser al menos 1.")]
        [SwaggerSchema(Description = "El salario ofrecido para la oferta de trabajo.")]
        public decimal Salary { get; set; }

        /// <summary>
        /// El tipo de contrato de la oferta de trabajo.
        /// </summary>
        [Required]
        [MinLength(5, ErrorMessage = "El tipo de contrato debe tener al menos 5 caracteres.")]
        [SwaggerSchema(Description = "El tipo de contrato de la oferta de trabajo.")]
        public string ContractType { get; set; } = string.Empty;
    }
}
