// Importazione dei modelli e delle librerie necessarie
using EcommerceLiveEfCore.Models; // Modelli personalizzati dell'applicazione
using Microsoft.AspNetCore.Identity; // Gestione utenti e ruoli con Identity
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Implementazione di Identity con Entity Framework Core
using Microsoft.EntityFrameworkCore; // Libreria per l'interazione con il database tramite Entity Framework Core

namespace EcommerceLiveEfCore.Data
{
    /*
        CONFIGURAZIONE DI ASP.NET IDENTITY CON ENTITY FRAMEWORK CORE

        La classe ApplicationDbContext eredita da IdentityDbContext<> e utilizza una serie di entità predefinite
        di ASP.NET Identity, combinate con classi personalizzate per la gestione degli utenti e dei ruoli.

        Le entità utilizzate sono:
        - ApplicationUser: Utente personalizzato (estende IdentityUser)
        - ApplicationRole: Ruolo personalizzato (estende IdentityRole)
        - string: Tipo della chiave primaria (nel nostro caso, utilizziamo una stringa, come gli ID GUID)
        - IdentityUserClaim<string>: Tabella delle "dichiarazioni" dell'utente (claim)
        - ApplicationUserRole: Relazione personalizzata molti-a-molti tra utenti e ruoli (estende IdentityUserRole)
        - IdentityUserLogin<string>: Gestisce i login esterni (es. Google, Facebook)
        - IdentityRoleClaim<string>: Contiene i claim associati ai ruoli
        - IdentityUserToken<string>: Memorizza eventuali token per l'utente (es. token di accesso OAuth)
    */

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, 
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        // Costruttore che riceve le opzioni di configurazione del database e le passa alla classe base
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Definizione delle tabelle nel database tramite DbSet<>
        
        // Tabella per gli utenti personalizzati
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Tabella per i ruoli personalizzati
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        // Tabella per la relazione tra utenti e ruoli
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        // Tabella per i prodotti
        public DbSet<Product> Products { get; set; }

        // Configurazione avanzata del modello durante la creazione del database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chiama la configurazione di IdentityDbContext per garantire che tutte le impostazioni di Identity siano applicate
            base.OnModelCreating(modelBuilder);

            // Configura la relazione tra ApplicationUserRole e ApplicationUser
            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.User) // Un ApplicationUserRole ha un utente associato
                .WithMany(u => u.ApplicationUserRole) // Un utente può avere più ruoli
                .HasForeignKey(ur => ur.UserId); // Definizione della chiave esterna che collega l'utente al ruolo
            
            // Configura la relazione tra ApplicationUserRole e ApplicationRole
            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.Role) // Un ApplicationUserRole ha un ruolo associato
                .WithMany(u => u.ApplicationUserRole) // Un ruolo può essere assegnato a più utenti
                .HasForeignKey(ur => ur.RoleId); // Definizione della chiave esterna che collega il ruolo all'utente

            modelBuilder.Entity<Product>().HasOne(p => p.User).WithMany(u => u.Products).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Product>().Property(p => p.UserId).IsRequired(false);
        }
    }
}
