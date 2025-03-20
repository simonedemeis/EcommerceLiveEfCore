using Microsoft.AspNetCore.Identity;

namespace EcommerceLiveEfCore.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> ApplicationUserRole { get; set; }
    }
}
