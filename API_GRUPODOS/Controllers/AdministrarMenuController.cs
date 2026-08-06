using API_GRUPODOS.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrarMenuController(IConfiguration _config) : ControllerBase
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

            return Ok(response);
        }

        [HttpPost("InsertarCategoriaAPI")]
        public IActionResult InsertarCategoriaAPI(CategoriaRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Descripcion", model.Descripcion);

            var response = context.Execute(
                "SP_InsertarCategoria",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Categoría registrada correctamente");

            return BadRequest("No se pudo registrar la categoría, verifica que no exista ya con ese nombre");
        }

        [HttpPut("ActualizarCategoriaAPI")]
        public IActionResult ActualizarCategoriaAPI(int idCategoria, CategoriaRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", idCategoria);
            parameters.Add("@Descripcion", model.Descripcion);

            var response = context.Execute(
                "SP_ActualizarCategoria",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Categoría actualizada correctamente");

            return BadRequest("No se pudo actualizar la categoría");
        }

        [HttpDelete("DesactivarCategoriaAPI")]
        public IActionResult DesactivarCategoriaAPI(int idCategoria)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", idCategoria);

            var response = context.Execute(
                "SP_DesactivarCategoria",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Categoría desactivada correctamente");

            return BadRequest("No se pudo desactivar la categoría");
        }
        [HttpGet("ConsultarCategoriasInactivasAPI")]
        public IActionResult ConsultarCategoriasInactivasAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<CategoriaProductoModel>(
                "SP_ConsultarCategoriasInactivas",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpPut("ReactivarCategoriaAPI")]
        public IActionResult ReactivarCategoriaAPI(int idCategoria)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", idCategoria);

            var response = context.Execute(
                "SP_ReactivarCategoria",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Categoría reactivada correctamente");

            return BadRequest("No se pudo reactivar la categoría");
        }

        #endregion

        #region Productos

        [HttpGet("ConsultarTodosLosProductosAPI")]
        public IActionResult ConsultarTodosLosProductosAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<ProductoAdminModel>(
                "SP_ConsultarTodosLosProductos",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpGet("ConsultarProductoPorIdAPI")]
        public IActionResult ConsultarProductoPorIdAPI(int idProducto)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);

            var response = context.QueryFirstOrDefault<ProductoModel>(
                "SP_ConsultarProductoPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response != null)
                return Ok(response);

            return BadRequest("No se encontró el producto");
        }

        [HttpPost("InsertarProductoAPI")]
        public IActionResult InsertarProductoAPI(ProductoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", model.IdCategoria);
            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@Descripcion", model.Descripcion);
            parameters.Add("@Precio", model.Precio);
            parameters.Add("@Imagen", model.Imagen);
            parameters.Add("@Stock", model.Stock);
            parameters.Add("@PuntosEsfuerzo", model.PuntosEsfuerzo);
            parameters.Add("@PedidoAnticipado", model.PedidoAnticipado);

            var response = context.Execute(
                "SP_InsertarProducto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Producto registrado correctamente");

            return BadRequest("No se pudo registrar el producto");
        }

        [HttpPut("ActualizarProductoAPI")]
        public IActionResult ActualizarProductoAPI(int idProducto, ProductoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);
            parameters.Add("@IdCategoria", model.IdCategoria);
            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@Descripcion", model.Descripcion);
            parameters.Add("@Precio", model.Precio);
            parameters.Add("@Imagen", model.Imagen);
            parameters.Add("@Stock", model.Stock);
            parameters.Add("@PuntosEsfuerzo", model.PuntosEsfuerzo);
            parameters.Add("@PedidoAnticipado", model.PedidoAnticipado);

            var response = context.Execute(
                "SP_ActualizarProducto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Producto actualizado correctamente");

            return BadRequest("No se pudo actualizar el producto");
        }

        [HttpDelete("DesactivarProductoAPI")]
        public IActionResult DesactivarProductoAPI(int idProducto)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);

            var response = context.Execute(
                "SP_DesactivarProducto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Producto desactivado correctamente");

            return BadRequest("No se pudo desactivar el producto");
        }
        [HttpGet("ConsultarProductosInactivosAPI")]
        public IActionResult ConsultarProductosInactivosAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<ProductoAdminModel>(
                "SP_ConsultarProductosInactivos",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpPut("ReactivarProductoAPI")]
        public IActionResult ReactivarProductoAPI(int idProducto)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);

            var response = context.Execute(
                "SP_ReactivarProducto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Producto reactivado correctamente");

            return BadRequest("No se pudo reactivar el producto");
        }

        #endregion

        #region Catálogo Semanal

        [HttpGet("ConsultarCatalogoSemanalAdminAPI")]
        public IActionResult ConsultarCatalogoSemanalAdminAPI(DateTime fechaInicioSemana)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@FechaInicioSemana", fechaInicioSemana);

            var response = context.Query<CatalogoSemanalAdminModel>(
                "SP_ConsultarCatalogoSemanalAdmin",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpPost("AgregarProductoCatalogoSemanalAPI")]
        public IActionResult AgregarProductoCatalogoSemanalAPI(AgregarCatalogoSemanalRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", model.IdProducto);
            parameters.Add("@FechaInicioSemana", model.FechaInicioSemana);
            parameters.Add("@StockDisponible", model.StockDisponible);
            parameters.Add("@LimitePorPersona", model.LimitePorPersona);

            var response = context.Execute(
                "SP_AgregarProductoCatalogoSemanal",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Producto agregado al catálogo semanal correctamente");

            return BadRequest("No se pudo agregar el producto al catálogo semanal");
        }

        [HttpPut("ActualizarCatalogoSemanalAPI")]
        public IActionResult ActualizarCatalogoSemanalAPI(int idCatalogoSemanal, ActualizarCatalogoSemanalRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCatalogoSemanal", idCatalogoSemanal);
            parameters.Add("@StockDisponible", model.StockDisponible);
            parameters.Add("@LimitePorPersona", model.LimitePorPersona);
            parameters.Add("@Activo", model.Activo);

            var response = context.Execute(
                "SP_ActualizarCatalogoSemanal",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Catálogo semanal actualizado correctamente");

            return BadRequest("No se pudo actualizar el catálogo semanal");
        }

        #endregion
    }


}