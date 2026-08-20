using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;


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
            return RedirectToAction("Principal", "Home");
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

                HttpContext.Session.SetInt32("IdUsuario", usuario!.IdUsuario);
                HttpContext.Session.SetString("Identificacion", usuario!.Identificacion);
                HttpContext.Session.SetString("Nombre", usuario!.NombreCompleto);
                HttpContext.Session.SetString("PrimerApellido", usuario!.PrimerApellido);
                HttpContext.Session.SetString("SegundoApellido", usuario!.SegundoApellido ?? string.Empty);
                HttpContext.Session.SetString("Email", usuario!.Email);
                HttpContext.Session.SetInt32("IdRol", usuario!.IdRol);
                HttpContext.Session.SetString("Token", usuario!.Token);
                if (usuario!.IdRol == 1)
                {
                    return RedirectToAction("Index", "AdministrarMenu");
                }
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
            return RedirectToAction("Principal", "Home");
        }

        #endregion

        #region Registrar

        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            ConsultarTiposDeIdentificacion();
            ConsultarGeneros();
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarUsuario(UsuarioRegistroModel model)
        {
            if (!ModelState.IsValid)
            {
                ConsultarTiposDeIdentificacion();
                ConsultarGeneros();
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
                ConsultarGeneros();
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

        private void ConsultarGeneros()
        {
            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + "Home/ConsultarGenerosAPI";

            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Generos = response.Content
                    .ReadFromJsonAsync<List<GeneroModel>>()
                    .Result;
            }
            else
            {
                ViewBag.Generos = new List<GeneroModel>();
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

        [HttpGet]
        public IActionResult DetallePedido(int idPedido)
        {
            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarDetallePedidoAPI?idPedido={idPedido}";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            var detalle = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<DetallePedidoModel>>().Result ?? new()
                : new List<DetallePedidoModel>();

            return View(detalle);
        }


        #endregion

        #region Gestion de Perfil

        [HttpGet]
        public IActionResult GestionPerfil()
        {
            string identificacion = HttpContext.Session.GetString("Identificacion") ?? string.Empty;

            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            var url = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";

            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            if (response.IsSuccessStatusCode)
            {
                ViewBag.InfoUsuario = response.Content
                    .ReadFromJsonAsync<UsuarioConsultaModel>()
                    .Result;
            }
            else
            {
                ViewBag.MensajePerfil = "La información del usuario no pudo cargarse";
                ViewBag.InfoUsuario = new UsuarioConsultaModel();
            }

            ViewBag.TienePedidoActivo = TienePedidoActivo(identificacion);
            ConsultarGeneros();

            return View();
        }

        [HttpPost]
        public IActionResult ActualizarPerfil(UsuarioEdicionRequestModel model)
        {
            string identificacion = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
            model.Identificacion = identificacion;

            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            // valido de nuevo en el servidor que no tenga pedido activo, no confio solo en el HTML deshabilitado del cliente
            if (TienePedidoActivo(identificacion))
            {
                ViewBag.MensajePerfil = "No puedes editar tu perfil mientras tengas un pedido activo";
                ViewBag.TienePedidoActivo = true;
                ConsultarGeneros();

                var urlConsultaBloqueado = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";
                var responseConsultaBloqueado = client.GetAsync(urlConsultaBloqueado).Result;

                ViewBag.InfoUsuario = responseConsultaBloqueado.IsSuccessStatusCode
                    ? responseConsultaBloqueado.Content.ReadFromJsonAsync<UsuarioConsultaModel>().Result
                    : new UsuarioConsultaModel();

                return View("GestionPerfil");
            }

            var url = _config["Valores:UrlApi"] + "Home/ActualizarPerfilAPI";
            var response = client.PutAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("Nombre", model.NombreCompleto);

                ViewBag.MensajePerfil = "Perfil actualizado correctamente";
            }
            else
            {
                ViewBag.MensajePerfil = response.Content.ReadAsStringAsync().Result;
            }

            var urlConsulta = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";
            var responseConsulta = client.GetAsync(urlConsulta).Result;

            ViewBag.InfoUsuario = responseConsulta.IsSuccessStatusCode
                ? responseConsulta.Content.ReadFromJsonAsync<UsuarioConsultaModel>().Result
                : new UsuarioConsultaModel();

            ViewBag.TienePedidoActivo = TienePedidoActivo(identificacion);
            ConsultarGeneros();

            return View("GestionPerfil");
        }

        [HttpPost]
        public IActionResult CambiarContrasenaPerfil(CambiarContrasenaPerfilModel model)
        {
            string identificacion = HttpContext.Session.GetString("Identificacion") ?? string.Empty;
            model.Identificacion = identificacion;

            using var clientConsulta = _http.CreateClient();
            clientConsulta.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            if (!ModelState.IsValid)
            {
                ViewBag.MensajeSeguridad = "Las contraseñas no coinciden o no cumplen los requisitos";
            }
            else
            {
                using var client = _http.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var url = _config["Valores:UrlApi"] + "Home/CambiarContrasenaPerfilAPI";
                var response = client.PutAsJsonAsync(url, model).Result;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("CerrarSesion", "Home");

                ViewBag.MensajeSeguridad = response.StatusCode == HttpStatusCode.OK
                    ? "Contraseña actualizada correctamente"
                    : response.Content.ReadAsStringAsync().Result;
            }

            var urlConsulta = _config["Valores:UrlApi"] + $"Home/ConsultarInformacionUsuarioAPI?identificacion={identificacion}";
            var responseConsulta = clientConsulta.GetAsync(urlConsulta).Result;

            ViewBag.InfoUsuario = responseConsulta.IsSuccessStatusCode
                ? responseConsulta.Content.ReadFromJsonAsync<UsuarioConsultaModel>().Result
                : new UsuarioConsultaModel();

            ConsultarGeneros();

            return View("GestionPerfil");
        }

        // consulto si el usuario tiene un pedido en estado Pendiente o En Preparacion (RF-03)
        private bool TienePedidoActivo(string identificacion)
        {
            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            var url = _config["Valores:UrlApi"] + $"Home/ValidarPedidoActivoAPI?identificacion={identificacion}";
            var response = client.GetAsync(url).Result;

            return response.IsSuccessStatusCode && response.Content.ReadFromJsonAsync<bool>().Result;
        }

        #endregion
    }
}