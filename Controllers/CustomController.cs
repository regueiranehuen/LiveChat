using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

        
    }
}
