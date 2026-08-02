using API_GRUPODOS.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController(IConfiguration _config) : ControllerBase
    {

        #region Categorías

        [HttpGet("ConsultarCategoriasAPI")]
        public IActionResult ConsultarCategoriasAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<CategoriaProductoModel>(
                "SP_ConsultarCategoriasProducto",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            if (response.Any())
                return Ok(response);

            return BadRequest("No se encontraron categorías registradas");
        }

        #endregion

        #region Productos

        [HttpGet("ConsultarProductosPorCategoriaAPI")]
        public IActionResult ConsultarProductosPorCategoriaAPI(int idCategoria)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            // calculo el lunes de esta semana, que es la fecha que usa el catalogo semanal
            var fechaInicioSemana = ObtenerLunesDeEstaSemana();

            var parameters = new DynamicParameters();
            parameters.Add("@FechaInicioSemana", fechaInicioSemana);
            parameters.Add("@IdCategoria", idCategoria);

            // ahora consulto el catalogo semanal, ya no el catalogo maestro directo
            var response = context.Query<ProductoCatalogoModel>(
                "SP_ConsultarCatalogoSemanalCliente",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            if (response.Any())
                return Ok(response);

            return BadRequest("No se encontraron productos para esa categoría");
        }

        #endregion

        // devuelve la fecha del lunes de la semana en la que estamos hoy
        private static DateTime ObtenerLunesDeEstaSemana()
        {
            var hoy = DateTime.Today;
            int diasDesdeElLunes = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;
            return hoy.AddDays(-1 * diasDesdeElLunes);
        }
    }
}