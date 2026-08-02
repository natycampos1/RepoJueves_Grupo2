using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PJ_GRUPODOS.Controllers
{
    public class AdministrarMenuController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {

        // Verifica que el usuario en sesión sea Administrador antes de cualquier acción
        private bool EsAdministrador()
        {
            int idRol = HttpContext.Session.GetInt32("IdRol") ?? 0;
            return idRol == 1;
        }

        // devuelve el lunes de la semana actual, la misma fecha que usa el catalogo semanal
        private static DateTime ObtenerLunesDeEstaSemana()
        {
            var hoy = DateTime.Today;
            int diasDesdeElLunes = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;
            return hoy.AddDays(-1 * diasDesdeElLunes);
        }

        #region Categorías

        [HttpGet]
        public IActionResult Index()
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();

            var url = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarCategoriasAPI";
            var response = client.GetAsync(url).Result;

            ViewBag.Categorias = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new List<CategoriaProductoModel>();

            var urlInactivas = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarCategoriasInactivasAPI";
            var responseInactivas = client.GetAsync(urlInactivas).Result;

            ViewBag.CategoriasInactivas = responseInactivas.IsSuccessStatusCode
                ? responseInactivas.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new List<CategoriaProductoModel>();

            return View();
        }

        [HttpPost]
        public IActionResult InsertarCategoria(CategoriaRequestModel model)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "AdministrarMenu/InsertarCategoriaAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCategoria"] = "La categoría se agregó correctamente";
                TempData["TipoMensajeCategoria"] = "success";
            }
            else
            {
                TempData["MensajeCategoria"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCategoria"] = "danger";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ActualizarCategoria(int idCategoria, CategoriaRequestModel model)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/ActualizarCategoriaAPI?idCategoria={idCategoria}";
            var response = client.PutAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCategoria"] = "La categoría se editó correctamente";
                TempData["TipoMensajeCategoria"] = "success";
            }
            else
            {
                TempData["MensajeCategoria"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCategoria"] = "danger";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DesactivarCategoria(int idCategoria)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/DesactivarCategoriaAPI?idCategoria={idCategoria}";
            var response = client.DeleteAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCategoria"] = "La categoría se eliminó correctamente";
                TempData["TipoMensajeCategoria"] = "success";
            }
            else
            {
                TempData["MensajeCategoria"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCategoria"] = "danger";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ReactivarCategoria(int idCategoria)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/ReactivarCategoriaAPI?idCategoria={idCategoria}";
            var response = client.PutAsync(url, null).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCategoria"] = "La categoría se reactivó correctamente";
                TempData["TipoMensajeCategoria"] = "success";
            }
            else
            {
                TempData["MensajeCategoria"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCategoria"] = "danger";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Productos

        [HttpGet]
        public IActionResult ProductosIndex()
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();

            var urlCategorias = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarCategoriasAPI";
            var responseCategorias = client.GetAsync(urlCategorias).Result;
            ViewBag.Categorias = responseCategorias.IsSuccessStatusCode
                ? responseCategorias.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new List<CategoriaProductoModel>();

            var urlProductos = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarTodosLosProductosAPI";
            var responseProductos = client.GetAsync(urlProductos).Result;
            ViewBag.Productos = responseProductos.IsSuccessStatusCode
                ? responseProductos.Content.ReadFromJsonAsync<List<ProductoAdminModel>>().Result ?? new()
                : new List<ProductoAdminModel>();

            var urlInactivos = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarProductosInactivosAPI";
            var responseInactivos = client.GetAsync(urlInactivos).Result;
            ViewBag.ProductosInactivos = responseInactivos.IsSuccessStatusCode
                ? responseInactivos.Content.ReadFromJsonAsync<List<ProductoAdminModel>>().Result ?? new()
                : new List<ProductoAdminModel>();

            return View();
        }

        [HttpPost]
        public IActionResult InsertarProducto(ProductoRequestModel model)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "AdministrarMenu/InsertarProductoAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeProducto"] = "El producto se agregó correctamente";
                TempData["TipoMensajeProducto"] = "success";
            }
            else
            {
                TempData["MensajeProducto"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeProducto"] = "danger";
            }

            return RedirectToAction("ProductosIndex");
        }

        [HttpGet]
        public IActionResult EditarProducto(int idProducto)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();

            var urlCategorias = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarCategoriasAPI";
            var responseCategorias = client.GetAsync(urlCategorias).Result;
            ViewBag.Categorias = responseCategorias.IsSuccessStatusCode
                ? responseCategorias.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new List<CategoriaProductoModel>();

            var urlProducto = _config["Valores:UrlApi"] + $"AdministrarMenu/ConsultarProductoPorIdAPI?idProducto={idProducto}";
            var responseProducto = client.GetAsync(urlProducto).Result;

            if (!responseProducto.IsSuccessStatusCode)
                return RedirectToAction("ProductosIndex");

            var producto = responseProducto.Content.ReadFromJsonAsync<ProductoModel>().Result;

            ViewBag.IdProducto = idProducto;

            return View(producto);
        }

        [HttpPost]
        public IActionResult EditarProducto(int idProducto, ProductoRequestModel model)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/ActualizarProductoAPI?idProducto={idProducto}";
            var response = client.PutAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeProducto"] = "El producto se editó correctamente";
                TempData["TipoMensajeProducto"] = "success";
            }
            else
            {
                TempData["MensajeProducto"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeProducto"] = "danger";
            }

            return RedirectToAction("ProductosIndex");
        }

        [HttpPost]
        public IActionResult DesactivarProducto(int idProducto)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/DesactivarProductoAPI?idProducto={idProducto}";
            var response = client.DeleteAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeProducto"] = "El producto se eliminó correctamente";
                TempData["TipoMensajeProducto"] = "success";
            }
            else
            {
                TempData["MensajeProducto"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeProducto"] = "danger";
            }

            return RedirectToAction("ProductosIndex");
        }

        [HttpPost]
        public IActionResult ReactivarProducto(int idProducto)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/ReactivarProductoAPI?idProducto={idProducto}";
            var response = client.PutAsync(url, null).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeProducto"] = "El producto se reactivó correctamente";
                TempData["TipoMensajeProducto"] = "success";
            }
            else
            {
                TempData["MensajeProducto"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeProducto"] = "danger";
            }

            return RedirectToAction("ProductosIndex");
        }

        #endregion
        #region Pedidos

        [HttpGet]
        public IActionResult PedidosIndex()
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "Pedido/ConsultarTodosLosPedidosAPI";
            var response = client.GetAsync(url).Result;

            var pedidos = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<PedidoAdminModel>>().Result ?? new()
                : new List<PedidoAdminModel>();

            return View(pedidos);
        }

        [HttpPost]
        public IActionResult ActualizarEstadoPedido(int idPedido, int idEstadoPedido)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"Pedido/ActualizarEstadoPedidoAPI?idPedido={idPedido}&idEstadoPedido={idEstadoPedido}";
            var response = client.PutAsync(url, null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["MensajePedidos"] = "Estado del pedido actualizado correctamente";
                TempData["TipoMensajePedidos"] = "success";
            }
            else
            {
                TempData["MensajePedidos"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajePedidos"] = "danger";
            }

            return RedirectToAction("PedidosIndex");
        }

        #endregion

        #region Catálogo Semanal

        [HttpGet]
        public IActionResult CatalogoSemanalIndex()
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            using var client = _http.CreateClient();

            // traigo los productos del catalogo maestro, para poder elegir cual agregar a la semana
            var urlProductos = _config["Valores:UrlApi"] + "AdministrarMenu/ConsultarTodosLosProductosAPI";
            var responseProductos = client.GetAsync(urlProductos).Result;
            ViewBag.Productos = responseProductos.IsSuccessStatusCode
                ? responseProductos.Content.ReadFromJsonAsync<List<ProductoAdminModel>>().Result ?? new()
                : new List<ProductoAdminModel>();

            // traigo lo que ya esta configurado para la semana actual
            var fechaInicioSemana = ObtenerLunesDeEstaSemana();
            var urlCatalogo = _config["Valores:UrlApi"] + $"AdministrarMenu/ConsultarCatalogoSemanalAdminAPI?fechaInicioSemana={fechaInicioSemana:yyyy-MM-dd}";
            var responseCatalogo = client.GetAsync(urlCatalogo).Result;

            var catalogoSemanal = responseCatalogo.IsSuccessStatusCode
                ? responseCatalogo.Content.ReadFromJsonAsync<List<CatalogoSemanalAdminModel>>().Result ?? new()
                : new List<CatalogoSemanalAdminModel>();

            ViewBag.FechaInicioSemana = fechaInicioSemana;

            return View(catalogoSemanal);
        }

        [HttpPost]
        public IActionResult AgregarCatalogoSemanal(int idProducto, int stockDisponible, int limitePorPersona)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            var model = new AgregarCatalogoSemanalRequestModel
            {
                IdProducto = idProducto,
                FechaInicioSemana = ObtenerLunesDeEstaSemana(),
                StockDisponible = stockDisponible,
                LimitePorPersona = limitePorPersona
            };

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + "AdministrarMenu/AgregarProductoCatalogoSemanalAPI";
            var response = client.PostAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCatalogoSemanal"] = "El producto se agregó al catálogo semanal correctamente";
                TempData["TipoMensajeCatalogoSemanal"] = "success";
            }
            else
            {
                TempData["MensajeCatalogoSemanal"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCatalogoSemanal"] = "danger";
            }

            return RedirectToAction("CatalogoSemanalIndex");
        }

        [HttpPost]
        public IActionResult ActualizarCatalogoSemanal(int idCatalogoSemanal, int stockDisponible, int limitePorPersona, bool activo)
        {
            if (!EsAdministrador())
                return RedirectToAction("Principal", "Home");

            var model = new ActualizarCatalogoSemanalRequestModel
            {
                StockDisponible = stockDisponible,
                LimitePorPersona = limitePorPersona,
                Activo = activo
            };

            using var client = _http.CreateClient();
            var url = _config["Valores:UrlApi"] + $"AdministrarMenu/ActualizarCatalogoSemanalAPI?idCatalogoSemanal={idCatalogoSemanal}";
            var response = client.PutAsJsonAsync(url, model).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["MensajeCatalogoSemanal"] = "El catálogo semanal se actualizó correctamente";
                TempData["TipoMensajeCatalogoSemanal"] = "success";
            }
            else
            {
                TempData["MensajeCatalogoSemanal"] = response.Content.ReadAsStringAsync().Result;
                TempData["TipoMensajeCatalogoSemanal"] = "danger";
            }

            return RedirectToAction("CatalogoSemanalIndex");
        }

        #endregion
    }
}