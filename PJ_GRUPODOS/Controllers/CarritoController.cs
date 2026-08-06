using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace PJ_GRUPODOS.Controllers
{
    public class CarritoController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {
        private const string ClaveSesionCarrito = "Carrito";

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

        #endregion

        #region Ver Carrito

        [HttpGet]
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);

            // ahora consulto el stock por IdCatalogoSemanal, no por IdProducto
            var stockDisponible = new Dictionary<int, int>();

            using var client = _http.CreateClient();
            foreach (var item in carrito)
            {
                var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarStockCatalogoSemanalAPI?idCatalogoSemanal={item.IdCatalogoSemanal}";
                var response = client.GetAsync(url).Result;

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
        public IActionResult Agregar(int idProducto, int idCatalogoSemanal, string nombreProducto, decimal precio, string? imagen, int limitePorPersona)
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
                // valido que no se pase del limite por persona antes de sumar
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

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Pedido/ConsultarTiposEntregaAPI";
            var response = client.GetAsync(url).Result;

            ViewBag.TiposEntrega = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<TipoEntregaModel>>().Result ?? new()
                : new List<TipoEntregaModel>();

            ViewBag.Carrito = carrito;
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarPedido(RegistrarPedidoModel model)
        {
            var carrito = ObtenerCarrito();

            if (!carrito.Any())
                return RedirectToAction("Index");

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
                // mando el IdCatalogoSemanal de cada item, la API lo necesita para descontar el stock semanal
                Carrito = carrito.Select(i => new { i.IdProducto, i.IdCatalogoSemanal, i.Cantidad }).ToList()
            };

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Pedido/RegistrarPedidoAPI";
            var response = client.PostAsJsonAsync(url, pedidoRequest).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.Session.Remove(ClaveSesionCarrito);
                TempData["MensajePedido"] = "¡Tu pedido se registró correctamente!";
                return RedirectToAction("MisPedidos", "Carrito");
            }

            ViewBag.MensajeError = response.Content.ReadAsStringAsync().Result;

            using var clientTipos = _http.CreateClient();
            var urlTipos = _config["Valores:UrlApi"] + "Pedido/ConsultarTiposEntregaAPI";
            var responseTipos = clientTipos.GetAsync(urlTipos).Result;

            ViewBag.TiposEntrega = responseTipos.IsSuccessStatusCode
                ? responseTipos.Content.ReadFromJsonAsync<List<TipoEntregaModel>>().Result ?? new()
                : new List<TipoEntregaModel>();

            ViewBag.Carrito = carrito;
            ViewBag.Total = carrito.Sum(i => i.Precio * i.Cantidad);

            return View();
        }

        #endregion

        #region Mis Pedidos

        [HttpGet]
        public IActionResult MisPedidos()
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarPedidosPorUsuarioAPI?idUsuario={idUsuario}";
            var response = client.GetAsync(url).Result;

            var pedidos = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<PedidoModel>>().Result ?? new()
                : new List<PedidoModel>();

            return View(pedidos);
        }

        [HttpGet]
        public IActionResult DetallePedido(int idPedido)
        {
            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarDetallePedidoAPI?idPedido={idPedido}";
            var response = client.GetAsync(url).Result;

            var detalle = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<DetallePedidoModel>>().Result ?? new()
                : new List<DetallePedidoModel>();

            return View(detalle);
        }

        #endregion
    }
}