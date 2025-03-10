using EcommerceLiveEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceLiveEfCore.Data
{
    public class EcommerceLiveEfCoreDbContext : DbContext
    {
        public EcommerceLiveEfCoreDbContext(DbContextOptions<EcommerceLiveEfCoreDbContext> options) : base(options) { }

        //definisco il dbset che rappresenta tabella sul database
        public DbSet<Product> Products { get; set; }
    }
}
