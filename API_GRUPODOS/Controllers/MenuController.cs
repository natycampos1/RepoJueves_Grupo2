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

            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", idCategoria);

            var response = context.Query<ProductoModel>(
                "SP_ConsultarProductosPorCategoria",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            if (response.Any())
                return Ok(response);

            return BadRequest("No se encontraron productos para esa categoría");
        }

        #endregion
    }
}