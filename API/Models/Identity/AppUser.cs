using Microsoft.AspNetCore.Identity;

namespace API.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}