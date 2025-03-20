// Importazione delle dipendenze necessarie
using EcommerceLiveEfCore.Data; // Contesto del database
using EcommerceLiveEfCore.Models; // Modelli personalizzati
using EcommerceLiveEfCore.Services; // Servizi personalizzati
using Microsoft.AspNetCore.Authentication.Cookies; // Middleware per l'autenticazione con i cookie
using Microsoft.AspNetCore.Identity; // Sistema di gestione utenti e ruoli
using Microsoft.EntityFrameworkCore; // Entity Framework Core per l'interazione con il database

// Creazione di un'istanza dell'applicazione web
var builder = WebApplication.CreateBuilder(args);

// Aggiunge il supporto per i controller e le viste MVC
builder.Services.AddControllersWithViews();

// Recupera la stringa di connessione dal file di configurazione (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura il contesto del database con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Configurazione di Identity con utenti e ruoli personalizzati
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Imposta se l'account deve essere confermato via email prima di poter accedere
    options.SignIn.RequireConfirmedAccount =
       builder.Configuration.GetSection("Identity").GetValue<bool>("RequireConfirmedAccount");

    // Imposta la lunghezza minima della password
    options.Password.RequiredLength =
        builder.Configuration.GetSection("Identity").GetValue<int>("RequiredLength");

    // Richiede che la password contenga almeno un numero
    options.Password.RequireDigit =
        builder.Configuration.GetSection("Identity").GetValue<bool>("RequireDigit");

    // Richiede almeno una lettera minuscola nella password
    options.Password.RequireLowercase =
        builder.Configuration.GetSection("Identity").GetValue<bool>("RequireLowercase");

    // Richiede almeno un carattere speciale nella password
    options.Password.RequireNonAlphanumeric =
        builder.Configuration.GetSection("Identity").GetValue<bool>("RequireNonAlphanumeric");

    // Richiede almeno una lettera maiuscola nella password
    options.Password.RequireUppercase =
        builder.Configuration.GetSection("Identity").GetValue<bool>("RequireUppercase");
})
    // Utilizza il contesto del database per archiviare utenti e ruoli
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // Aggiunge provider di token predefiniti per la gestione delle autenticazioni e conferme
    .AddDefaultTokenProviders();

//CONFIGURAZIONE DELL'AUTENTICAZIONE CON I COOKIE
// Le proprietà DefaultAuthenticateScheme e DefaultChallengeScheme vengono utilizzate per definire
// come il sistema di autenticazione gestisce le richieste e le sfide di autenticazione
builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Schema di autenticazione predefinito
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Schema per le sfide di autenticazione
    })
    .AddCookie(options => {
        options.LoginPath = "/Account/Login"; // Percorso della pagina di login
        options.AccessDeniedPath = "/Account/Login"; // Pagina di accesso negato
        options.Cookie.HttpOnly = true; // Impedisce l'accesso ai cookie tramite JavaScript per motivi di sicurezza
        /*options.ExpireTimeSpan = TimeSpan.FromSeconds(60);*/ // Durata della sessione di autenticazione
        //options.SlidingExpiration = true;
        options.Cookie.Name = "EcommerceLiveEfCore"; // Nome del cookie per l'autenticazione
    });

// 📌 REGISTRAZIONE DEI SERVIZI PERSONALIZZATI NEL CONTAINER DI DEPENDENCY INJECTION
builder.Services.AddScoped<ProductService>(); // Servizio per la gestione dei prodotti
builder.Services.AddScoped<LoggerService>(); // Servizio per la gestione dei log
builder.Services.AddScoped<UserManager<ApplicationUser>>(); // Servizio per la gestione degli utenti
builder.Services.AddScoped<SignInManager<ApplicationUser>>(); // Servizio per la gestione dell'accesso degli utenti
builder.Services.AddScoped<RoleManager<ApplicationRole>>(); // Servizio per la gestione dei ruoli

// Inizializza il sistema di logging personalizzato
LoggerService.ConfigureLogger();

// Costruisce l'applicazione
var app = builder.Build();

//CONFIGURAZIONE DELLA PIPELINE DI GESTIONE DELLE RICHIESTE HTTP

// Se l'applicazione non è in modalità sviluppo
if (!app.Environment.IsDevelopment())
{
    // Abilita un gestore di errori globale che reindirizza alla pagina di errore
    app.UseExceptionHandler("/Home/Error");
    
    // Abilita l'HTTP Strict Transport Security (HSTS) per forzare connessioni sicure
    app.UseHsts();
}

// Forza l'uso di HTTPS per le richieste in entrata
app.UseHttpsRedirection();

// Abilita il supporto per i file statici (CSS, JS, immagini, ecc.)
app.UseStaticFiles();

// Configura il sistema di routing per gestire le richieste HTTP
app.UseRouting();

// Abilita il middleware per la gestione dell'autenticazione degli utenti
app.UseAuthentication();

// Abilita il middleware per la gestione dell'autorizzazione degli utenti autenticati
app.UseAuthorization();

// Configura il routing predefinito per i controller e le azioni
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Route predefinita

// Avvia l'applicazione e inizia ad ascoltare le richieste HTTP
app.Run();
