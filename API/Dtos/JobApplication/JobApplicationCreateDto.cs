using API.Dtos.JobApplication.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Dtos.JobApplication
{
    /// <summary>
    /// Dto para creación de aplicaciones a ofertas de trabajo (sin Id, JobOffer y DateApplied)
    /// </summary>
    [SwaggerSchema(Description = "Dto para la creación de una nueva aplicación a oferta de trabajo.")]
    public class JobApplicationCreateDto : JobApplicationBaseDto
    {
    }
}
