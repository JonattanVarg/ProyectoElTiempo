using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos.Identity;
using API.Dtos.Responses;
using API.Enums.Identity;
using API.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController:ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<AppUser> userManager,
            ILogger<AccountController> logger,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="registerDto">El objeto que contiene los datos de registro del usuario.</param>
        /// <returns>Un objeto <see cref="AuthResponseDto"/> indicando el éxito o fracaso del registro.</returns>
        /// <response code="200">Registro exitoso. Devuelve un mensaje de éxito.</response>
        /// <response code="400">Solicitud inválida. Devuelve errores de validación o problemas específicos del registro.</response>
        /// <response code="500">Error interno del servidor. Ocurrió un problema inesperado durante el registro.</response>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registra un nuevo usuario", Description = "Permite registrar un nuevo usuario en el sistema con un rol específico. El registro es controlado por el rol del usuario actual.")]
        [SwaggerResponse(200, "Registro exitoso", typeof(AuthResponseDto))]
        [SwaggerResponse(400, "Solicitud inválida o errores de validación", typeof(AuthResponseDto))]
        [SwaggerResponse(500, "Error interno del servidor", typeof(AuthResponseDto))]
        public async Task<ActionResult<string>> Register(
            [FromBody][SwaggerParameter(Description = "Los datos necesarios para registrar un nuevo usuario. Incluye correo electrónico, nombre completo, contraseña y rol.")]
            RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Solicitud de registro con modelo no válido para el correo electrónico {Email}", registerDto.Email);
                return BadRequest(ModelState);
            }
            
            try
            {
                // Verificar si ya existe un usuario con el mismo correo electrónico
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUserByEmail != null)
                {
                    _logger.LogWarning("Intento de registro fallido: El correo electrónico {Email} ya está en uso.", registerDto.Email);
                    return Conflict(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "El correo electrónico ya está en uso."
                    });
                }

                var user = new AppUser { Email = registerDto.Email, FullName = registerDto.FullName, UserName = registerDto.Email };

                _logger.LogInformation("Creando usuario para el correo electrónico {Email}", registerDto.Email);

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Error al crear el usuario para el correo electrónico {Email}: {Errors}", registerDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest(result.Errors);
                }

                // El Rol de registro es Candidato
                string role = UserRole.Candidato.ToString();

                _logger.LogInformation("Asignando rol {Role} al usuario con correo electrónico {Email}", role, registerDto.Email);

                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Error al asignar el rol {Role} al usuario con correo electrónico {Email}: {Errors}", role, registerDto.Email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    
                    // Si la asignación del rol falla, eliminar el usuario creado para evitar inconsistencias
                    await _userManager.DeleteAsync(user);
                    return BadRequest(roleResult.Errors);
                }

                _logger.LogInformation("Registro exitoso para el usuario con correo electrónico {Email}", registerDto.Email);

                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Cuenta creada exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el registro del usuario con correo electrónico {Email}", registerDto.Email);
                return StatusCode(500, new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }

        /// <summary>
        /// Inicia sesión y devuelve un token JWT.
        /// </summary>
        /// <param name="loginDto">El objeto que contiene el correo electrónico y la contraseña del usuario.</param>
        /// <returns>Un objeto <see cref="AuthResponseDto"/> que contiene un token JWT si las credenciales son válidas; de lo contrario, un mensaje de error.</returns>
        /// <response code="200">Inicio de sesión exitoso. Devuelve un token JWT.</response>
        /// <response code="400">Solicitud inválida. El modelo de datos no es válido.</response>
        /// <response code="401">No autorizado. Las credenciales proporcionadas son incorrectas.</response>
        /// <response code="500">Error interno del servidor. Ocurrió un problema inesperado durante el inicio de sesión.</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Inicia sesión y devuelve un token JWT",
            Description = "Proporciona un correo electrónico y una contraseña válidos para obtener un token JWT, que permite acceder a la aplicación como Admin, Reclutador o Candidato.")]
        [SwaggerResponse(200, "Inicio de sesión exitoso", typeof(AuthResponseDto))]
        [SwaggerResponse(400, "Solicitud inválida", typeof(AuthResponseDto))]
        [SwaggerResponse(401, "Credenciales inválidas", typeof(AuthResponseDto))]
        [SwaggerResponse(500, "Error interno del servidor", typeof(AuthResponseDto))]
        public async Task<ActionResult<AuthResponseDto>> Login(
            [FromBody][SwaggerParameter(Description = "Los datos de inicio de sesión, incluyendo el correo electrónico y la contraseña del usuario.")]
            LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Solicitud de login con modelo no válido para el correo electrónico {Email}", loginDto.Email);
                    return BadRequest(ModelState);
                }

                // Validar que el usuario este registrado por el correo - En el program.cs se clarifica que cada usuario debe tener un email único
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                _logger.LogInformation("Intento de login para el correo electrónico: {Email}", loginDto.Email);

                if (user is null)
                {
                    _logger.LogWarning("Error de inicio de sesión: usuario no encontrado con correo electrónico {Email}", loginDto.Email);
                    // Para proteger contra ataques de fuerza bruta, no revelamos si el usuario o la contraseña son incorrectos.
                    return Unauthorized(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Credenciales inválidas"
                    });
                }

                // Aquí verifico si la cuenta está bloqueada
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Usuario con correo electrónico {Email} bloqueado debido a múltiples intentos fallidos de inicio de sesión", loginDto.Email);
                    return Unauthorized(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Su cuenta ha sido bloqueada debido a múltiples intentos fallidos. Inténtelo de nuevo más tarde."
                    });
                }

                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var token = GenerateToken(user);

                    _logger.LogInformation("Inicio de sesión exitoso para el correo electrónico: {Email}", loginDto.Email);

                    return Ok(new AuthResponseDto
                    {
                        Token = token,
                        IsSuccess = true,
                        Message = "Inicio de sesión exitoso."
                    });
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("Usuario con correo electrónico {Email} bloqueado después de intentos fallidos de inicio de sesión", loginDto.Email);
                    return Unauthorized(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Su cuenta ha sido bloqueada debido a múltiples intentos fallidos. Inténtelo de nuevo más tarde."
                    });
                }
                else
                {
                    _logger.LogWarning("Error de inicio de sesión: contraseña no válida para el correo electrónico {Email}", loginDto.Email);
                    return Unauthorized(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Credenciales inválidas"
                    });
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el inicio de sesión para el correo electrónico {Email}", loginDto.Email);
                return StatusCode(500, new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }


        private string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);


            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Email,user.Email ?? ""),
                new(JwtRegisteredClaimNames.Name,user.FullName ?? ""),
                new(JwtRegisteredClaimNames.NameId,user.Id ?? ""),
                new(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JWTSetting").GetSection("ValidAudience").Value!),
                new(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JWTSetting").GetSection("ValidIssuer").Value!)
            ];

            var roles = _userManager.GetRolesAsync(user).Result;
            
            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
            
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios del sistema con detalles básicos.
        /// </summary>
        /// <returns>
        /// Una lista de objetos <see cref="UserInfo"/> que representan los usuarios del sistema.
        /// </returns>
        /// <response code="200">La solicitud fue exitosa y se devuelve la lista de usuarios.</response>
        /// <response code="401">No autorizado. El usuario no tiene el rol adecuado para acceder a esta información.</response>
        /// <response code="500">Error interno del servidor. Ocurrió un problema inesperado al intentar obtener la lista de usuarios.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
        {
            try
            {
                var users = await _userManager.Users.Select(u => new UserInfo
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FullName = u.FullName!,
                    Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault()!
                }).ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al obtener la lista de usuarios.");
                return StatusCode(500, "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}