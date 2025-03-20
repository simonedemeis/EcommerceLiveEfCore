using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EcommerceLiveEfCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public DateOnly? BirthDate { get; set; }

        public ICollection<ApplicationUserRole> ApplicationUserRole { get; set; }
        
        public ICollection<Product> Products { get; set; }
    }
}
