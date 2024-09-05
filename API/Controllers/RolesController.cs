using API.Dtos.Identity.Remaining;
using API.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRolDto createRolDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var roleExist = await _roleManager.RoleExistsAsync(createRolDto.RolName);

            if(roleExist) return BadRequest("Rol already exits");

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRolDto.RolName));

            if(roleResult.Succeeded) return Ok( new {message = "Role created successfully"});

            return BadRequest("Role created failed");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _roleManager.Roles.Select(r => new RoleResponseDto()
            {
                Id = r.Id,
                Name = r.Name,
                TotalUsers = _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
            }).ToListAsync();

            return Ok(roles);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role is null) return NotFound("Role not found.");

            var result = await _roleManager.DeleteAsync(role);

            if(result.Succeeded) return Ok(new {message = "Role deleted successfully"});

            return BadRequest("Role deletion failed");
        }

        [HttpPost("assign")]
        public async Task<ActionResult> AssignRole([FromBody] RoleAssignDto roleAssignDto)
        {
            var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);

            if (user is null) return NotFound("User not found");

            var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId);

            if (role is null) return NotFound("Role not found");

            // Obtener los roles actuales del usuario
            var userRoles = await _userManager.GetRolesAsync(user);

            // Si el usuario ya tiene el rol asignado, no hacer nada
            if (userRoles != null && userRoles.Contains(role.Name!))
                return Ok(new { message = "Usuario ya tiene el rol asignado" });

            // Eliminar todos los roles anteriores del usuario
            if (userRoles != null && userRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                {
                    var error = removeResult.Errors.FirstOrDefault();
                    return BadRequest(error?.Description ?? "Error removing existing roles");
                }
            }

            // Asignar el nuevo rol
            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if (result.Succeeded)
                return Ok(new { message = "Role asignado satisfactoriamente" });

            var assignError = result.Errors.FirstOrDefault();

            return BadRequest(assignError?.Description ?? "Error assigning role");
        }
    }
}