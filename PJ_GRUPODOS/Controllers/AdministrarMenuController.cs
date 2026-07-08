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
    }
}