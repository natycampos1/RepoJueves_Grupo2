using PJ_GRUPODOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace PJ_GRUPODOS.Controllers
{
    public class ChatController(
        IHttpClientFactory _http,
        IConfiguration _config) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            var url = _config["Valores:UrlApi"] + "Pedido/ConsultarPedidosChatAPI";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToAction("CerrarSesion", "Home");

            List<PedidoChatModel> pedidos = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<List<PedidoChatModel>>().Result ?? new()
                : new();

            ViewBag.Token = HttpContext.Session.GetString("Token");
            ViewBag.UrlHub = _config["Valores:UrlHub"];
            ViewBag.IdUsuario = HttpContext.Session.GetInt32("IdUsuario");

            return View(pedidos);
        }

        [HttpGet]
        public IActionResult ConsultarMensajes(int idPedido)
        {
            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

            var url = _config["Valores:UrlApi"] + $"Pedido/ConsultarMensajesAPI?idPedido={idPedido}";
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Forbid();

            var json = response.Content.ReadAsStringAsync().Result;
            return Content(json, "application/json");
        }
    }
}