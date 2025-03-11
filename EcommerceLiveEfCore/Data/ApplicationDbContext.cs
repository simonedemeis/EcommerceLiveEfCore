using EcommerceLiveEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceLiveEfCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
