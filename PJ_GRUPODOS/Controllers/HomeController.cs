using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;


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
            if (TempData["MensajeLogin"] != null)
                ViewBag.Mensaje = TempData["MensajeLogin"];

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
                InfoVariableSesionUsuarioModel? usuario = response.Content.ReadFromJsonAsync<InfoVariableSesionUsuarioModel>().Result;

                HttpContext.Session.SetInt32("Autenticado", 1);

                HttpContext.Session.SetString("Identificacion", usuario!.Identificacion);
                HttpContext.Session.SetString("Nombre", usuario!.NombreCompleto);
                HttpContext.Session.SetString("PrimerApellido", usuario!.PrimerApellido);
                HttpContext.Session.SetString("SegundoApellido", usuario!.SegundoApellido ?? string.Empty);
                HttpContext.Session.SetString("Email", usuario!.Email);
                HttpContext.Session.SetInt32("IdRol", usuario!.IdRol);

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
            return RedirectToAction("Index", "Home");
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
            if (!ModelState.IsValid)
            {
                ConsultarTiposDeIdentificacion();
                return View(model);
            }

            //Creo el cliente http
            using var client = _http.CreateClient();
            //Obtengo la url del api
            var url = _config["Valores:UrlApi"] + "Home/RegistrarUsuarioAPI";
            //Envio el modelo al api y tomo la respuesta
            var response = client.PostAsJsonAsync(url, model).Result;

            //Manejo la respeuesta recibida del api
            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeLogin"] = "Cuenta creada correctamente, inicia sesión para continuar";
                return RedirectToAction("IniciarSesion", "Home");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ConsultarTiposDeIdentificacion();
                ViewBag.Mensaje = response.Content.ReadAsStringAsync().Result;
                return View();
            }
            throw new Exception("Error al registrar cliente");
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
            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Home/RecuperarAccesoAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.Mensaje = "Se ha generado una contraseña temporal, revisa tu correo electrónico";
                return View();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Mensaje = response.Content.ReadAsStringAsync().Result;
                return View();
            }

            throw new Exception("Error al recuperar el acceso");
        }

        #endregion
        #region Principal
        public IActionResult Principal()
        {
            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Menu/ConsultarCategoriasAPI";
            var response = client.GetAsync(url).Result;

            ViewBag.Categorias = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new List<CategoriaProductoModel>();

            return View();
        }
        #endregion

        #region Páginas Informativas

        [HttpGet]
        public IActionResult SobreNosotros()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NuestroEquipo()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NuestroServicio()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Testimonios()
        {
            return View();
        }

        #endregion

        #region Gestion de Perfil

        [HttpGet]
        public IActionResult GestionPerfil()
        {
            string identificacion = HttpContext.Session.GetString("Identificacion") ?? string.Empty;

            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";

            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                ViewBag.InfoUsuario = response.Content
                    .ReadFromJsonAsync<UsuarioConsultaModel>()
                    .Result;
            }
            else
            {
                ViewBag.Mensaje = "La información del usuario no pudo cargarse";
                ViewBag.InfoUsuario = new UsuarioConsultaModel();
            }

            return View();
        }


        [HttpPost]
        public IActionResult GestionPerfil(UsuarioEdicionRequestModel model)
        {
            string identificacion = HttpContext.Session.GetString("Identificacion") ?? string.Empty;

            if (!ModelState.IsValid)
            {
                ViewBag.Mensaje = "Las contraseñas no coinciden";

                using var clientValidacion = _http.CreateClient();
                var urlConsultaValidacion = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";
                var responseValidacion = clientValidacion.GetAsync(urlConsultaValidacion).Result;

                ViewBag.InfoUsuario = responseValidacion.IsSuccessStatusCode
                    ? responseValidacion.Content.ReadFromJsonAsync<UsuarioConsultaModel>().Result
                    : new UsuarioConsultaModel();

                return View();
            }

            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + "Home/ActualizarUsuarioAPI";

            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.Mensaje = "Perfil actualizado correctamente";
            }
            else
            {
                ViewBag.Mensaje = response.Content.ReadAsStringAsync().Result;
            }

            // Volver a consultar la info actualizada para repoblar la vista
            var urlConsulta = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";
            var responseConsulta = client.GetAsync(urlConsulta).Result;

            if (responseConsulta.IsSuccessStatusCode)
            {

                ViewBag.InfoUsuario = responseConsulta.Content
                    .ReadFromJsonAsync<UsuarioConsultaModel>()
                    .Result;
            }
            else
            {
                ViewBag.InfoUsuario = new UsuarioConsultaModel();
            }
            HttpContext.Session.SetInt32("Autenticado", 1);
            return View();
        }

        #endregion
    }
}