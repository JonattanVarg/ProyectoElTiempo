using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Dtos.Identity
{
    public class RegisterDto
    {
        /// <summary>
        /// El correo electrónico del usuario.
        /// </summary>
        [Required]
        [EmailAddress]
        [SwaggerSchema("El correo electrónico del usuario a registrar")]
        [SwaggerParameter(Description = "El correo electrónico del usuario a registrar")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [SwaggerSchema("El nombre completo del usuario a registrar")]
        [SwaggerParameter(Description = "El nombre completo del usuario a registrar")]

        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// La contraseña del usuario, que debe incluir al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.
        /// </summary>
        [Required]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]{8,}$",
                            ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        [SwaggerSchema("La contraseña del usuario, que debe incluir al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;
    }
}