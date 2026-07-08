using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PJ_GRUPODOS.Controllers
{
    public class ContactoController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {

        #region Contáctanos

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(MensajeContactoModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Contacto/RegistrarMensajeAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.Mensaje = "Tu mensaje fue enviado correctamente, te contactaremos pronto";
                return View();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ViewBag.Mensaje = "No se pudo enviar el mensaje, intenta nuevamente";
                return View(model);
            }

            throw new Exception("Error al enviar el mensaje de contacto");
        }

        #endregion
    }
}