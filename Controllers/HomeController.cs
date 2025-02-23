using System.Diagnostics;
using System.Security.Claims;
using LiveChat.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LiveChat.Controllers
{
    public class HomeController : CustomController
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        
        public IActionResult Login(string usuario)
        {

            Debug.WriteLine("USERNAME JUJU: " + usuario);

            // Aquí puedes guardar la cookie o realizar la autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Conversaciones");  // Redirigir al chat después de loguearse

        }

        public IActionResult Chatroom(string usuario)
        {
            if (string.IsNullOrEmpty(usuario))
            {
                Debug.WriteLine("El usuario actual es null o vacío");
                return RedirectToAction("Index");
            }

            Debug.WriteLine("Nombre usuario actual: " + usuario);
            TempData["usuario"] = usuario;

            return View();
        }

        public IActionResult Conversaciones()
        {
            return ControlarLogueo();
        }

        public IActionResult Chat(string conversacion)
        {

            if (LogueoHecho())
            {
                ViewBag.Usuario = User.Identity.Name;
                TempData["idConversacion"] = conversacion;
                return View();
            }
            return RedirectToAction("Index");

           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
