using System.Security.Claims;
using EcommerceLiveEfCore.Models;
using EcommerceLiveEfCore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceLiveEfCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager
        )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return PartialView("_LoginForm");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // Trova l'utente in base all'email fornita nel modello di login
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            // Se l'utente non esiste, restituisce la vista senza effettuare l'accesso
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Email or password is incorrect"
                });
            }

            // Prova a effettuare il login utilizzando il gestore di autenticazione
            // Il terzo parametro (true) indica che il login persistente è attivato
            // Il quarto parametro (false) indica che il blocco dell'account non viene considerato in caso di tentativi falliti
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, true, false);

            if (!signInResult.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = "Email or password is incorrect"
                });
            }
            
            // Recupera i ruoli associati all'utente autenticato
            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            // Crea una lista di claims (dichiarazioni) da associare all'utente autenticato
            List<Claim> claims = new List<Claim>();

            // Aggiunge il claim con il nome dell'utente
            claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));

            // Aggiunge il claim con l'email dell'utente
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            // Aggiunge i ruoli dell'utente come claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crea un'identità basata sui claim con lo schema di autenticazione dei cookie
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Effettua l'autenticazione dell'utente e memorizza le sue informazioni nei cookie di autenticazione
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Reindirizza l'utente alla pagina principale dopo un accesso riuscito
            return Json(new
            {
                success = true,
                message = "Login Successful"
            });
        }

        public IActionResult Register()
        {
            return PartialView("_RegisterForm");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            // Crea un nuovo oggetto ApplicationUser e inizializza i suoi campi con i valori forniti dal modello di registrazione
            var newUser = new ApplicationUser()
            {
                Email = registerViewModel.Email, // Imposta l'email dell'utente
                UserName = registerViewModel.Email, // Imposta il nome utente uguale all'email
                FirstName = registerViewModel.FirstName, // Imposta il nome dell'utente
                LastName = registerViewModel.LastName, // Imposta il cognome dell'utente
                BirthDate = registerViewModel.BirthDate, // Imposta la data di nascita dell'utente
            };

            // Crea l'utente nel database con la password specificata
            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            // Se la creazione dell'utente non ha avuto successo, reindirizza alla pagina dei prodotti
            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = result.Errors
                });
            }

            // Trova l'utente appena creato in base alla sua email
            var user = await _userManager.FindByEmailAsync(newUser.Email);

            // Aggiunge l'utente al ruolo "User"
             await _userManager.AddToRoleAsync(user, "User");

            // Reindirizza alla pagina dei prodotti dopo la registrazione completata con successo
            return Json(new
            {
                success = true,
                message = "User created successfully!"
            });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}