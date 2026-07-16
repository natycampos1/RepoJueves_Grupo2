using API_GRUPODOS.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController(IConfiguration _config) : ControllerBase
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

        #region Registrar Pedido

        [HttpPost("RegistrarPedidoAPI")]
        public IActionResult RegistrarPedidoAPI(RegistrarPedidoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
            context.Open();

            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1. Validar que haya stock suficiente para TODOS los productos antes de continuar
                    foreach (var item in model.Carrito)
                    {
                        var parametrosStock = new DynamicParameters();
                        parametrosStock.Add("@IdProducto", item.IdProducto);
                        var stockDisponible = context.QueryFirstOrDefault<ProductoStockModel>(
                            "SP_ConsultarStockProducto",
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

                    var idPedido = context.QuerySingle<int>(
                        "SP_RegistrarPedido",
                        parametrosPedido,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    // 4. Registrar cada línea de detalle y descontar el stock
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
                        parametrosStock.Add("@IdProducto", item.IdProducto);
                        parametrosStock.Add("@Cantidad", item.Cantidad);

                        context.Execute(
                            "SP_DescontarStock",
                            parametrosStock,
                            commandType: System.Data.CommandType.StoredProcedure
                        );
                    }

                    // 5. Si todo salió bien, confirmamos la transacción completa
                    scope.Complete();

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
    }
}