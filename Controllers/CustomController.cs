using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LiveChat.Controllers
{
    public class CustomController : Controller
    {

        


        public IActionResult ControlarLogueo()
        {
            var usuario = User.Identity.Name;
            
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Index");
            }
            Debug.WriteLine("Usuario autenticado: " + usuario);
            ViewBag.Usuario = usuario;
            return View();
        }

        public bool LogueoHecho()
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return false;
            }
            return true;
        }

        public bool ConversacionExistenteYCorrespondiente(string idConversacion) 
        {
            string[] usuarios = idConversacion.Split(',');
            // Evitar viveza con JS
            MongoDBConnection m = new MongoDBConnection();
            ConversacionRepository c = new ConversacionRepository(m);
            ConversacionHub cH = new ConversacionHub(c);

            string usuarioAutenticado = User.Identity.Name;

            if (!usuarioAutenticado.Equals(usuarios[0]) && !usuarioAutenticado.Equals(usuarios[1]))
            {
                return false; // si bien puede existir la conversacion, no es incumbencia del usuario que me mandó javascript si es así
            }




            return cH.ExisteConversacion(usuarios[0], usuarios[1]) != null;

        }



        
    }
}
