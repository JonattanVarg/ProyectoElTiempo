using System.ComponentModel.DataAnnotations;

namespace API.Dtos.Identity.Remaining
{
    public class CreateRolDto
    {
        [Required(ErrorMessage = "Role Name is required.")]
        [MinLength(1, ErrorMessage = "Role Name cannot be an empty string.")]
        public string RolName { get; set; }
    }
}