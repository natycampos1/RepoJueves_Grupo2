using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PJ_GRUPODOS.Controllers
{
    public class CarritoController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {
        private const string ClaveSesionCarrito = "Carrito";

        // un pedido con menos de 100 puntos es Pequeño, 100 o mas es Grande (RF-13)
        private const int LimitePuntosPedidoGrande = 100;

        #region Utilidades internas del carrito

        private List<ItemCarritoModel> ObtenerCarrito()
        {
            var json = HttpContext.Session.GetString(ClaveSesionCarrito);

            if (string.IsNullOrEmpty(json))
                return new List<ItemCarritoModel>();

            return JsonSerializer.Deserialize<List<ItemCarritoModel>>(json) ?? new();
        }

        private void GuardarCarrito(List<ItemCarritoModel> carrito)
        {
            var json = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString(ClaveSesionCarrito, json);
        }

        // armo un HttpClient con el token de sesion ya puesto, para no repetir esto en cada metodo
        private HttpClient CrearClienteAutenticado()
        {
            var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            return client;
        }

        // calculo los puntos totales y clasifico el pedido (RF-12/RF-13)
        private (int puntosTotales, bool esPedidoGrande, bool tieneProductoAnticipado) ClasificarPedido(List<ItemCarritoModel> carrito)
        {
            int puntosTotales = carrito.Sum(i => i.PuntosEsfuerzo * i.Cantidad);
            bool tieneProductoAnticipado = carrito.Any(i => i.PedidoAnticipado);
            bool esPedidoGrande = puntosTotales >= LimitePuntosPedidoGrande || tieneProductoAnticipado;

            return (puntosTotales, esPedidoGrande, tieneProductoAnticipado);
        }

        // valido la fecha y hora de entrega elegidas contra las reglas del calendario (RF-14)
        private string? ValidarFechaEntrega(DateTime fecha, TimeSpan hora, bool esPedidoGrande)
        {
            var fechaHoraElegida = fecha.Date + hora;

            if (fechaHoraElegida <= DateTime.Now)
                return "La fecha y hora de entrega deben ser posteriores al momento actual";

            if (esPedidoGrande)
            {
                if (fechaHoraElegida < DateTime.Now.AddHours(48))
                    return "Los pedidos grandes o con productos anticipados requieren al menos 48 horas de anticipación";

                if (fecha.DayOfWeek == DayOfWeek.Sunday)
                    return "No se puede entregar los domingos";
            }
            else
            {
                if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                    return "Los pedidos pequeños solo se pueden entregar de lunes a viernes";

                if (hora < new TimeSpan(8, 0, 0) || hora > new TimeSpan(17, 0, 0))
                    return "El horario de entrega debe ser entre las 8:00 a.m. y las 5:00 p.m.";
            }

            return null;
        }

        #endregion

        #region Ver Carrito

        [HttpGet]
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);

            var stockDisponible = new Dictionary<int, int>();

            using var client = CrearClienteAutenticado();
            foreach (var item in carrito)
            {
                var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarStockCatalogoSemanalAPI?idCatalogoSemanal={item.IdCatalogoSemanal}";
                var response = client.GetAsync(url).Result;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("CerrarSesion", "Home");

                if (response.IsSuccessStatusCode)
                {
                    var stock = response.Content.ReadFromJsonAsync<ProductoStockModel>().Result;
                    stockDisponible[item.IdCatalogoSemanal] = stock?.Stock ?? 0;
                }
            }

            ViewBag.StockDisponible = stockDisponible;

            return View(carrito);
        }

        #endregion

        #region Agregar al Carrito

        [HttpPost]
        public IActionResult Agregar(int idProducto, int idCatalogoSemanal, string nombreProducto, decimal precio, string? imagen, int limitePorPersona, int puntosEsfuerzo, bool pedidoAnticipado)
        {
            var autenticado = HttpContext.Session.GetInt32("Autenticado") == 1;
            if (!autenticado)
            {
                return RedirectToAction("IniciarSesion", "Home");
            }

            var carrito = ObtenerCarrito();

            var itemExistente = carrito.FirstOrDefault(i => i.IdProducto == idProducto);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad + 1 > limitePorPersona)
                {
                    TempData["MensajeCarrito"] = $"Solo puedes llevar un máximo de {limitePorPersona} unidades de '{nombreProducto}'";
                    return RedirectToAction("Index", "Menu");
                }

                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Add(new ItemCarritoModel
                {
                    IdProducto = idProducto,
                    IdCatalogoSemanal = idCatalogoSemanal,
                    NombreProducto = nombreProducto,
                    Precio = precio,
                    Cantidad = 1,
                    LimitePorPersona = limitePorPersona,
                    PuntosEsfuerzo = puntosEsfuerzo,
                    PedidoAnticipado = pedidoAnticipado,
                    Imagen = imagen
                });
            }

            GuardarCarrito(carrito);

            TempData["MensajeCarrito"] = $"'{nombreProducto}' se agregó al carrito";

            return RedirectToAction("Index", "Menu");
        }
        #endregion

        #region Actualizar Cantidad

        [HttpPost]
        public IActionResult ActualizarCantidad(int idProducto, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.IdProducto == idProducto);

            if (item != null)
            {
                if (cantidad <= 0)
                {
                    carrito.Remove(item);
                }
                else if (cantidad > item.LimitePorPersona)
                {
                    TempData["MensajeCarrito"] = $"Solo puedes llevar un máximo de {item.LimitePorPersona} unidades de '{item.NombreProducto}'";
                }
                else
                {
                    item.Cantidad = cantidad;
                }
            }

            GuardarCarrito(carrito);

            return RedirectToAction("Index");
        }

        #endregion

        #region Eliminar del Carrito

        [HttpPost]
        public IActionResult Eliminar(int idProducto)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.IdProducto == idProducto);

            if (item != null)
                carrito.Remove(item);

            GuardarCarrito(carrito);

            return RedirectToAction("Index");
        }

        #endregion

        #region Confirmar Pedido

        [HttpGet]
        public IActionResult ConfirmarPedido()
        {
            var carrito = ObtenerCarrito();

            if (!carrito.Any())
                return RedirectToAction("Index");

            using var client = CrearClienteAutenticado();
            var url = _config["Valores:UrlApi"] + "Pedido/ConsultarTiposEntregaAPI";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            ViewBag.TiposEntrega = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<TipoEntregaModel>>().Result ?? new()
                : new List<TipoEntregaModel>();

            var (puntosTotales, esPedidoGrande, tieneProductoAnticipado) = ClasificarPedido(carrito);

            ViewBag.Carrito = carrito;
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);
            ViewBag.PuntosTotales = puntosTotales;
            ViewBag.EsPedidoGrande = esPedidoGrande;
            ViewBag.TieneProductoAnticipado = tieneProductoAnticipado;

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarPedido(RegistrarPedidoModel model)
        {
            var carrito = ObtenerCarrito();

            if (!carrito.Any())
                return RedirectToAction("Index");

            var (puntosTotales, esPedidoGrande, tieneProductoAnticipado) = ClasificarPedido(carrito);

            string? errorFecha = null;

            if (!TimeSpan.TryParse(model.HoraEntrega, out var horaEntrega))
            {
                errorFecha = "La hora de entrega no es válida";
            }
            else
            {
                errorFecha = ValidarFechaEntrega(model.FechaEntrega, horaEntrega, esPedidoGrande);
            }

            if (errorFecha != null)
            {
                using var clientRecarga = CrearClienteAutenticado();
                var urlTiposRecarga = _config["Valores:UrlApi"] + "Pedido/ConsultarTiposEntregaAPI";
                var responseTiposRecarga = clientRecarga.GetAsync(urlTiposRecarga).Result;

                if (responseTiposRecarga.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("CerrarSesion", "Home");

                ViewBag.TiposEntrega = responseTiposRecarga.IsSuccessStatusCode
                    ? responseTiposRecarga.Content.ReadFromJsonAsync<List<TipoEntregaModel>>().Result ?? new()
                    : new List<TipoEntregaModel>();

                ViewBag.MensajeError = errorFecha;
                ViewBag.Carrito = carrito;
                ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);
                ViewBag.EsPedidoGrande = esPedidoGrande;
                ViewBag.TieneProductoAnticipado = tieneProductoAnticipado;

                return View();
            }

            var fechaEntregaProgramada = model.FechaEntrega.Date + horaEntrega;

            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            string email = HttpContext.Session.GetString("Email") ?? string.Empty;
            string nombre = HttpContext.Session.GetString("Nombre") ?? string.Empty;

            var pedidoRequest = new
            {
                IdUsuario = idUsuario,
                Email = email,
                NombreCliente = nombre,
                IdTipoEntrega = model.IdTipoEntrega,
                DireccionEntrega = model.DireccionEntrega,
                FechaEntregaProgramada = fechaEntregaProgramada,
                Carrito = carrito.Select(i => new { i.IdProducto, i.IdCatalogoSemanal, i.Cantidad }).ToList()
            };

            using var client = CrearClienteAutenticado();
            var url = _config["Valores:UrlApi"] + "Pedido/RegistrarPedidoAPI";
            var response = client.PostAsJsonAsync(url, pedidoRequest).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.Session.Remove(ClaveSesionCarrito);
                TempData["MensajePedido"] = "¡Tu pedido se registró correctamente!";
                return RedirectToAction("MisPedidos", "Carrito");
            }

            ViewBag.MensajeError = response.Content.ReadAsStringAsync().Result;

            using var clientTipos = CrearClienteAutenticado();
            var urlTipos = _config["Valores:UrlApi"] + "Pedido/ConsultarTiposEntregaAPI";
            var responseTipos = clientTipos.GetAsync(urlTipos).Result;

            ViewBag.TiposEntrega = responseTipos.IsSuccessStatusCode
                ? responseTipos.Content.ReadFromJsonAsync<List<TipoEntregaModel>>().Result ?? new()
                : new List<TipoEntregaModel>();

            ViewBag.Carrito = carrito;
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);
            ViewBag.EsPedidoGrande = esPedidoGrande;
            ViewBag.TieneProductoAnticipado = tieneProductoAnticipado;

            return View();
        }

        #endregion

        #region Mis Pedidos

        [HttpGet]
        public IActionResult MisPedidos()
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            using var client = CrearClienteAutenticado();
            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarPedidosPorUsuarioAPI?idUsuario={idUsuario}";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            var pedidos = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<PedidoModel>>().Result ?? new()
                : new List<PedidoModel>();

            return View(pedidos);
        }

        [HttpGet]
        public IActionResult DetallePedido(int idPedido)
        {
            using var client = CrearClienteAutenticado();
            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarDetallePedidoAPI?idPedido={idPedido}";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            var detalle = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<DetallePedidoModel>>().Result ?? new()
                : new List<DetallePedidoModel>();

            return View(detalle);
        }

        [HttpPost]
        public IActionResult CancelarPedido(int idPedido)
        {
            using var client = CrearClienteAutenticado();
            var url = _config["Valores:UrlApi"] + $"Pedido/CancelarPedidoClienteAPI?idPedido={idPedido}";
            var response = client.DeleteAsync(url).Result;

            return Json(response.Content.ReadAsStringAsync().Result);
        }

        #endregion
    }
}