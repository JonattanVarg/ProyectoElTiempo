using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Dtos.Identity
{
    public class LoginDto
    {
        /// <summary>
        /// El correo electrónico del usuario.
        /// </summary>
        [Required]
        [EmailAddress]
        [SwaggerSchema("El correo electrónico del usuario.")]
        [SwaggerParameter(Description = "Correo electrónico del usuario")]
        public string Email { get; set; } = string.Empty;

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