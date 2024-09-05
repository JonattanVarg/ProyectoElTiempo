    using API.Configurations.InitialUsers;
    using API.Configurations.InitialUsers.Interfaces;
    using API.Enums.Identity;
    using API.Models.Identity;
    using API.Validations;
    using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public static class AppDbInitializer
    {
        public static async Task Initialize(UserManager<AppUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            InitialUsersConfig initialUsersConfig,
                                            ILogger logger)
        {
            try
            {
                logger.LogInformation("Inicio del proceso de inicialización de la base de datos.");

                // Crea roles
                var roles = new List<IdentityRole>();
                foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
                {
                    roles.Add(new IdentityRole { Name = role.ToString(), NormalizedName = role.ToString().ToUpper() });
                }
                foreach (var role in roles)
                {
                    try
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name!))
                        {
                            var result = await roleManager.CreateAsync(role);

                            if (result.Succeeded)
                                logger.LogInformation("Rol {RoleName} creado exitosamente.", role.Name);
                            else
                                logger.LogWarning("Error al crear el rol {RoleName}: {Errors}", role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error al verificar o crear el rol {RoleName}.", role.Name);
                        throw;
                    }
                }

                // Creo los tres usuarios iniciales (Admin, Reclutador, Candidato)
                await CreateUserIfNotExists(userManager, initialUsersConfig.InitialAdministrador, logger);
                await CreateUserIfNotExists(userManager, initialUsersConfig.InitialReclutador, logger);
                await CreateUserIfNotExists(userManager, initialUsersConfig.InitialCandidato, logger);

                logger.LogInformation("Finalización del proceso de inicialización de la base de datos.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocurrió un error durante la inicialización de la base de datos.");
                throw;
            }
        }

        // Aqui se crea un usuario si no existe
        private static async Task CreateUserIfNotExists(UserManager<AppUser> userManager,
                                                        IInitialUser initialUser,
                                                        ILogger logger)
        {
            try
            {
                // Validar configuración de usuario inicial
                if (!InitialUserValidator.Validate(initialUser))
                {
                    logger.LogWarning("Configuración inválida para el usuario {Username}.", initialUser.Username);
                    throw new InvalidOperationException($"Configuración para el usuario {initialUser.Username} es inválida");
                }

                // Verificar si el usuario ya existe en la base de datos
                var user = await userManager.FindByEmailAsync(initialUser.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = initialUser.Username,
                        Email = initialUser.Email,
                        FullName = initialUser.FullName
                    };

                    // Crear el usuario y asignar el rol
                    var result = await userManager.CreateAsync(user, initialUser.Password);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Usuario {Username} creado exitosamente.", initialUser.Username);

                        var roleResult = await userManager.AddToRoleAsync(user, initialUser.Role);
                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation("Rol {Role} asignado exitosamente al usuario {Username}.", initialUser.Role, initialUser.Username);
                        }
                        else
                        {
                            logger.LogWarning("Error al asignar el rol {Role} al usuario {Username}: {Errors}", initialUser.Role, initialUser.Username, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogWarning("Error al crear el usuario {Username}: {Errors}", initialUser.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Usuario {Username} ya existe. Se omite la creación.", initialUser.Username);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocurrió un error al crear o asignar roles al usuario {Username}.", initialUser.Username);
                throw;
            }
        }
    }
}