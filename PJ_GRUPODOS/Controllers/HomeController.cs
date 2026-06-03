using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace PJ_GRUPODOS.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string CorreoElectronico, string Contrasenna)
        {
            return RedirectToAction("Principal", "Home");
        }

        #region Registrar

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(UsuarioModel model)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region RecuperarAcceso

        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarAcceso(string CorreoElectronico)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion

        public IActionResult Principal()
        {
            return View();
        }

    }
}