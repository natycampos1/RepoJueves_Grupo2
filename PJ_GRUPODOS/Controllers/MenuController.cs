using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace PJ_GRUPODOS.Controllers
{
    public class MenuController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {

        #region Menú

        [HttpGet]
        public IActionResult Index()
        {
            using var client = _http.CreateClient();

            // 1. Consulto las categorías
            var urlCategorias = _config["Valores:UrlApi"] + "Menu/ConsultarCategoriasAPI";
            var responseCategorias = client.GetAsync(urlCategorias).Result;

            List<CategoriaProductoModel> categorias = responseCategorias.IsSuccessStatusCode
                ? responseCategorias.Content.ReadFromJsonAsync<List<CategoriaProductoModel>>().Result ?? new()
                : new();

            // 2. Por cada categoría, consulto sus productos del catalogo semanal
            var productosPorCategoria = new Dictionary<int, List<ProductoCatalogoModel>>();

            foreach (var categoria in categorias)
            {
                var urlProductos = _config["Valores:UrlApi"] + $"Menu/ConsultarProductosPorCategoriaAPI?idCategoria={categoria.IdCategoria}";
                var responseProductos = client.GetAsync(urlProductos).Result;

                productosPorCategoria[categoria.IdCategoria] = responseProductos.IsSuccessStatusCode
                    ? responseProductos.Content.ReadFromJsonAsync<List<ProductoCatalogoModel>>().Result ?? new()
                    : new();
            }

            ViewBag.Categorias = categorias;
            ViewBag.ProductosPorCategoria = productosPorCategoria;

            return View();
        }

        #endregion
    }
}