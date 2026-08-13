using API_GRUPODOS.Models;
using API_GRUPODOS.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Transactions;


namespace API_GRUPODOS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController(IConfiguration _config, IUtilesService _utiles) : ControllerBase
    {

        #region Tipos de Entrega

        [HttpGet("ConsultarTiposEntregaAPI")]
        public IActionResult ConsultarTiposEntregaAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<TipoEntregaModel>(
                "SP_ConsultarTiposEntrega",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        #endregion

        #region Estados
        [HttpGet("ConsultarEstadosPedidoAPI")]
        public IActionResult ConsultarEstadosPedidoAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<EstadoPedidoModel>(
                "SP_ConsultarEstadosPedido",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }
        #endregion
        #region Registrar Pedido

        [HttpPost("RegistrarPedidoAPI")]
        public async Task<IActionResult> RegistrarPedidoAPI(RegistrarPedidoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
            context.Open();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 1. Validar que haya stock semanal suficiente para TODOS los productos antes de continuar
                    foreach (var item in model.Carrito)
                    {
                        var parametrosStock = new DynamicParameters();
                        parametrosStock.Add("@IdCatalogoSemanal", item.IdCatalogoSemanal);
                        var stockDisponible = context.QueryFirstOrDefault<ProductoStockModel>(
                            "SP_ConsultarStockCatalogoSemanal",
                            parametrosStock,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        if (stockDisponible == null || stockDisponible.Stock < item.Cantidad)
                            return BadRequest($"No hay stock suficiente para uno de los productos seleccionados");
                    }

                    // 2. Consultar el precio actual de cada producto y calcular el total
                    var preciosProductos = new Dictionary<int, decimal>();
                    decimal total = 0;

                    foreach (var item in model.Carrito)
                    {
                        var parametrosProducto = new DynamicParameters();
                        parametrosProducto.Add("@IdProducto", item.IdProducto);
                        var producto = context.QueryFirstOrDefault<ProductoModel>(
                            "SP_ConsultarProductoPorId",
                            parametrosProducto,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        if (producto == null)
                            return BadRequest("Uno de los productos seleccionados ya no está disponible");

                        preciosProductos[item.IdProducto] = producto.Precio;
                        total += producto.Precio * item.Cantidad;
                    }

                    // 3. Registrar el encabezado del pedido
                    var parametrosPedido = new DynamicParameters();
                    parametrosPedido.Add("@IdUsuario", model.IdUsuario);
                    parametrosPedido.Add("@IdTipoEntrega", model.IdTipoEntrega);
                    parametrosPedido.Add("@DireccionEntrega", model.DireccionEntrega);
                    parametrosPedido.Add("@Total", total);
                    parametrosPedido.Add("@FechaEntregaProgramada", model.FechaEntregaProgramada);

                    var idPedido = context.QuerySingle<int>(
                        "SP_RegistrarPedido",
                        parametrosPedido,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    // 4. Registrar cada línea de detalle y descontar el stock semanal
                    foreach (var item in model.Carrito)
                    {
                        var parametrosDetalle = new DynamicParameters();
                        parametrosDetalle.Add("@IdPedido", idPedido);
                        parametrosDetalle.Add("@IdProducto", item.IdProducto);
                        parametrosDetalle.Add("@Cantidad", item.Cantidad);
                        parametrosDetalle.Add("@PrecioUnitario", preciosProductos[item.IdProducto]);

                        context.Execute(
                            "SP_RegistrarDetallePedido",
                            parametrosDetalle,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        var parametrosStock = new DynamicParameters();
                        parametrosStock.Add("@IdCatalogoSemanal", item.IdCatalogoSemanal);
                        parametrosStock.Add("@Cantidad", item.Cantidad);

                        context.Execute(
                            "SP_DescontarStockSemanal",
                            parametrosStock,
                            commandType: System.Data.CommandType.StoredProcedure
                        );
                    }

                    // 5. Si todo salió bien, confirmamos la transacción completa
                    scope.Complete();

                    // 6. Enviar correo de confirmación (fuera de la lógica crítica de la transacción)
                    string ruta = Path.Combine(AppContext.BaseDirectory, "Templates", "PedidoConfirmado.html");
                    string plantilla = System.IO.File.ReadAllText(ruta);

                    plantilla = plantilla.Replace("{{NOMBRE}}", model.NombreCliente);
                    plantilla = plantilla.Replace("{{NUMEROPEDIDO}}", idPedido.ToString());
                    plantilla = plantilla.Replace("{{TOTAL}}", total.ToString("N0"));

                    await _utiles.EnviarCorreoAsync(model.Email, "Pedido Confirmado - RF Bakery", plantilla);

                    return Ok(idPedido);
                }
                catch (Exception)
                {
                    // Si algo falla, TransactionScope revierte automáticamente todo lo anterior
                    return BadRequest("No se pudo registrar el pedido, intente nuevamente");
                }
            }
        }

        #endregion

        #region Consultar Pedidos

        [HttpGet("ConsultarPedidosPorUsuarioAPI")]
        public IActionResult ConsultarPedidosPorUsuarioAPI(int idUsuario)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdUsuario", idUsuario);

            var response = context.Query<PedidoModel>(
                "SP_ConsultarPedidosPorUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpGet("ConsultarDetallePedidoAPI")]
        public IActionResult ConsultarDetallePedidoAPI(int idPedido)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdPedido", idPedido);

            var response = context.Query<DetallePedidoModel>(
                "SP_ConsultarDetallePedido",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [HttpGet("ConsultarTodosLosPedidosAPI")]
        public IActionResult ConsultarTodosLosPedidosAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<PedidoAdminModel>(
                "SP_ConsultarTodosLosPedidos",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        #endregion

        #region Administrar Estado de Pedido

        [HttpPut("ActualizarEstadoPedidoAPI")]
        public IActionResult ActualizarEstadoPedidoAPI(int idPedido, int idEstadoPedido)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdPedido", idPedido);
            parameters.Add("@IdEstadoPedido", idEstadoPedido);

            var response = context.Execute(
                "SP_ActualizarEstadoPedido",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Estado del pedido actualizado correctamente");

            return BadRequest("No se pudo actualizar el estado del pedido");
        }

        #endregion
        #region Stock

        [HttpGet("ConsultarStockProductoAPI")]
        public IActionResult ConsultarStockProductoAPI(int idProducto)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);

            var response = context.QueryFirstOrDefault<ProductoStockModel>(
                "SP_ConsultarStockProducto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response != null)
                return Ok(response);

            return NotFound("Producto no encontrado");
        }

        [HttpGet("ConsultarStockCatalogoSemanalAPI")]
        public IActionResult ConsultarStockCatalogoSemanalAPI(int idCatalogoSemanal)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdCatalogoSemanal", idCatalogoSemanal);

            var response = context.QueryFirstOrDefault<ProductoStockModel>(
                "SP_ConsultarStockCatalogoSemanal",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response != null)
                return Ok(response);

            return NotFound("No se encontró el item del catálogo semanal");
        }

        #endregion
        #region Chat

        [HttpGet("ConsultarPedidosChatAPI")]
        public IActionResult ConsultarPedidosChatAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdUsuario", _utiles.ObtenerConsecutivoToken());
            parameters.Add("@IdRol", _utiles.ObtenerIdRolToken());

            var response = context.Query<PedidoChatModel>("SP_ConsultarPedidosChat", parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(response);
        }

        [HttpGet("ConsultarMensajesAPI")]
        public IActionResult ConsultarMensajesAPI(int idPedido)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parametrosAcceso = new DynamicParameters();
            parametrosAcceso.Add("@IdPedido", idPedido);
            parametrosAcceso.Add("@IdUsuario", _utiles.ObtenerConsecutivoToken());
            parametrosAcceso.Add("@IdRol", _utiles.ObtenerIdRolToken());

            var tieneAcceso = context.QueryFirstOrDefault<int?>("SP_ValidarAccesoChatPedido", parametrosAcceso,
                commandType: System.Data.CommandType.StoredProcedure);

            if (tieneAcceso is null or 0)
                return Forbid();

            var parameters = new DynamicParameters();
            parameters.Add("@IdPedido", idPedido);
            var response = context.Query<MensajeResponseModel>("SP_ConsultarMensajesPedido", parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(response);
        }

        #endregion
    }
}