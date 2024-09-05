using API.Dtos.JobOffer.Base;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos.JobOffer
{
    /// <summary>
    /// Dto para actualización de ofertas de trabajo (sin Id, JobApplications y DatePosted)
    /// </summary>
    [SwaggerSchema(Description = "DTO para la actualización de una oferta de trabajo existente.")]

    public class JobOfferUpdateDto : JobOfferBaseDto 
    { 
    }
}
