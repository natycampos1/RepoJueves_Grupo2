using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace PJ_GRUPODOS.Controllers
{
    public class HomeController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {
        #region Index

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Iniciar Sesión

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(LoginModel model)
        {
            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Home/IniciarSesionAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //Obtengo la info del usuario
                UsuarioRegistroModel? usuario = response.Content.ReadFromJsonAsync<UsuarioRegistroModel>().Result;

                HttpContext.Session.SetString("Autenticado", "1");

                HttpContext.Session.SetString("Nombre", usuario!.NombreCompleto);
                HttpContext.Session.SetString("Identificacion", usuario!.Identificacion);
                HttpContext.Session.SetString("Email", usuario!.Email);

                return RedirectToAction("Principal", "Home");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                //Mensaje
                ViewBag.Mensaje = response.Content.ReadAsStringAsync().Result;
                return View();
            }

            throw new Exception("Error al iniciar sesión");
        }

        #endregion

        #region Cerrar Sesión

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }

        #endregion

        #region Registrar

        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            ConsultarTiposDeIdentificacion();
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarUsuario(UsuarioRegistroModel model)
        {
            //Creo el cliente http
            using var client = _http.CreateClient();
            //Obtengo la url del api
            var url = _config["Valores:UrlApi"] + "Home/RegistrarUsuarioAPI";
            //Envio el modelo al api y tomo la respuesta
            var response = client.PostAsJsonAsync(url, model).Result;

            //Manejo la respeuesta recibida del api
            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.Mensaje = "Cliente ingresado correctamente";
                return RedirectToAction("Principal", "Home");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ConsultarTiposDeIdentificacion();
                ViewBag.Mensaje = response.Content.ReadAsStringAsync().Result;
                return View();
            }
            throw new Exception("Error al registrar cliente");

            return View();
        }


        //Funciones internas para traer datos de tablas catalogo y presentar en el formulario de registro con dropdowns
        private void ConsultarTiposDeIdentificacion()
        {
            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + "Home/ConsultarTiposDeIdentificacionAPI";

            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                ViewBag.TiposDeIdentificacion = response.Content
                    .ReadFromJsonAsync<List<TipoIdentificacionModel>>()
                    .Result;
            }
            else
            {
                ViewBag.TiposDeIdentificacion = new List<TipoIdentificacionModel>();
            }
        }

        #endregion

        #region RecuperarAcceso

        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarAcceso(RecuperarAccesoModel model)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Principal
        public IActionResult Principal()
        {
            return View();
        }
        #endregion
    }
}