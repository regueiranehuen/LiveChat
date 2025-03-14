using System.Diagnostics;
using System.Security.Claims;
using LiveChat.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity.Data;

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
            /*if (LogueoHecho())
            {
                return RedirectToAction("Conversaciones"); // no tengo que iniciar sesión de nuevo si es que ya se guardó mi usuario en cookies
            }*/

            Logout();
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }


        /*
         
        HTTP POST VS HTTP GET

        HTTP GET 
        Sirve para obtener datos del servidor.
        Es idempotente (no cambia el estado del servidor).
        Los datos van en la URL (pueden verse en el navegador).
        No es seguro para información sensible (como contraseñas).
         

        HTTP POST
        Sirve para enviar datos al servidor (como formularios o información en JSON).
        Puede modificar o crear recursos en el servidor.
        Los datos van en el cuerpo de la solicitud (no en la URL).
        Más seguro que GET para datos sensibles.
         
        */



        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) // FromBody convierte el Json enviado en inicioSesion en un objeto LoginRequest
        {
            Debug.WriteLine("USERNAME JUJU: " + request.User);


            // Por contrato, cualquier tipo que se use dentro de un bloque using debe implementar la interfaz IDisposable.

            // Interfaz
            /*En C#, una interfaz (interface) es un contrato que define un conjunto de métodos, propiedades, eventos o indexadores que una clase debe implementar. Sin embargo, no contiene implementación, solo la definición de los métodos.
*/
            using (MongoDBConnection m = new MongoDBConnection())
            {
                UsuarioRepository u = new UsuarioRepository(m);
                UsuarioHub uH = new UsuarioHub(u);

                string inicioSesion = await uH.IniciarSesion(request.User, request.Password, request.Token, request.ConId);

                if (inicioSesion.Equals("Inicio de sesión exitoso"))
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, request.User) }; // Claim: dato que representa al usuario autenticado

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // Identidad del usuario con sus claims
                    var principal = new ClaimsPrincipal(identity); // Representa al usuario autenticado

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); // Guarda la sesión en una cookie para que el usuario no tenga que iniciar sesión en cada request
                                                                                                                
                    return Ok(true); // axios.post devuelve objeto de respuesta. Devuelve un status 200 OK con el valor true
                }
                else if (inicioSesion.Equals("El usuario no existe") || inicioSesion.Equals("La contraseña es incorrecta"))
                {
                    return Unauthorized("El usuario no existe / la contraseña es incorrecta");
                }
                else if (inicioSesion.Equals("Verificar captcha"))
                {
                    return Unauthorized("Verificar captcha");
                }

                return Unauthorized(inicioSesion); // Crear captcha
            }
            
           

        }

        public void Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            if (LogueoHecho() && ConversacionExistenteYCorrespondiente(conversacion)) // Chequeo si existe el id de conversacion ya que puede ser que el cliente esté modificando el id en la url
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
