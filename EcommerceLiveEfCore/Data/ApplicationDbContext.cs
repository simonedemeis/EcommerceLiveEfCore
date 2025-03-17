using EcommerceLiveEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceLiveEfCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired(true);
        }
    }
}
