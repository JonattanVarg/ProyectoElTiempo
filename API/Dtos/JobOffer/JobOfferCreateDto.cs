using API.Dtos.JobOffer.Base;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace API.Dtos.JobOffer
{
    /// <summary>
    /// Dto para creación de ofertas de trabajo (sin Id, JobApplications y DatePosted)
    /// </summary>
    [SwaggerSchema(Description = "DTO para la creación de una nueva oferta de trabajo.")]
    public class JobOfferCreateDto : JobOfferBaseDto
    {
       
    }
}
